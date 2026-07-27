using System.Net;
using DD.Shared.Details.Abstractions.Dto;
using DD.TerminalClient.Details.Api;
using DD.TerminalClient.Domain.Abstractions;
using DD.TerminalClient.Domain.Realtime;
using Microsoft.Extensions.Logging;

namespace DD.TerminalClient.Details.Realtime;

// Owns one hub connection and its manual reconnect loop, turning every raw connectivity condition into
// TaskHubEvent values for the application's event sink. It never mutates application or UI state - it only
// enqueues events. Reconnect backoff follows TerminalRetryPolicy; the first successful connect emits no
// event (startup loads the initial snapshot itself), while every later reconnect emits Reconnected so the
// application reloads. Server pushes that arrive while a snapshot load is in progress are buffered in
// arrival order and released only when the application drains them after reconciliation.
internal sealed class TaskHubClient(
    ITaskHubConnectionFactory connectionFactory,
    Func<string?> tokenProvider,
    Action<TaskHubEvent> eventSink,
    ILogger<TaskHubClient> logger,
    Func<TimeSpan, CancellationToken, Task>? delayAsync = null) : ITaskHubClient
{
    private readonly ITaskHubConnectionFactory _connectionFactory =
        connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    private readonly Func<string?> _tokenProvider =
        tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));

    private readonly Action<TaskHubEvent> _eventSink =
        eventSink ?? throw new ArgumentNullException(nameof(eventSink));

    private readonly ILogger<TaskHubClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly Func<TimeSpan, CancellationToken, Task> _delayAsync = delayAsync ?? DefaultDelayAsync;

    private readonly List<TaskHubEvent> _buffer = [];
    private readonly CancellationTokenSource _lifetimeCts = new();
    private readonly object _gate = new();

    private ITaskHubConnection? _connection;
    private CancellationTokenSource? _runCts;
    private TaskCompletionSource? _reconnectWake;
    private bool _started;
    private bool _shouldBeConnected;
    private bool _buffering;
    private bool _intentionalStop;
    private bool _reconnectLoopRunning;
    private int _reconnectAttempt;
    private bool _disposed;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ITaskHubConnection connection;
        CancellationToken runToken;
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_started)
            {
                return;
            }

            _started = true;
            _shouldBeConnected = true;
            _buffering = true;
            _reconnectAttempt = 0;
            _runCts = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeCts.Token);
            runToken = _runCts.Token;
            _connection = _connectionFactory.Create(_tokenProvider);
            connection = _connection;
        }

        connection.OnUpdate(HandleUpdate);
        connection.OnHeartbeat(HandleHeartbeat);
        connection.OnClosed(HandleConnectionClosedAsync);

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(runToken, cancellationToken);
        var outcome = await TryConnectAsync(linked.Token);

        if (outcome == TaskHubConnectOutcome.Unauthorized)
        {
            SetShouldBeConnected(false);
            Emit(TaskHubEvent.Unauthorized);
        }
        else if (outcome == TaskHubConnectOutcome.Failed)
        {
            StartReconnectLoop();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ITaskHubConnection? connection;
        CancellationTokenSource? runCts;
        lock (_gate)
        {
            if (_disposed || !_started || !_shouldBeConnected)
            {
                return;
            }

            _shouldBeConnected = false;
            _intentionalStop = true;
            connection = _connection;
            runCts = _runCts;
            _reconnectWake?.TrySetResult();
        }

        if (runCts is not null)
        {
            await runCts.CancelAsync();
        }

        try
        {
            if (connection is not null)
            {
                await connection.StopAsync(cancellationToken);
            }
        }
        finally
        {
            lock (_gate)
            {
                _intentionalStop = false;

                // Reset the start/run state so a later StartAsync (for example after a 401 and a
                // subsequent re-login) builds a fresh connection and reconnects, instead of returning
                // early on the stale _started guard and leaving the client permanently disconnected.
                _started = false;
                _connection = null;
                _runCts = null;
            }

            if (connection is not null)
            {
                await connection.DisposeAsync();
            }

            runCts?.Dispose();
        }

        Log.HubClosed(_logger);
        Emit(TaskHubEvent.Closed);
    }

    public async Task ReconnectNowAsync(CancellationToken cancellationToken)
    {
        ITaskHubConnection connection;
        CancellationToken runToken;
        bool loopRunning;
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_shouldBeConnected || _connection is null || _runCts is null)
            {
                return;
            }

            _reconnectAttempt = 0;
            loopRunning = _reconnectLoopRunning;
            connection = _connection;
            runToken = _runCts.Token;
            if (loopRunning)
            {
                // A reconnect loop is already waiting; waking it makes it retry without the backoff delay.
                _reconnectWake?.TrySetResult();
            }
            else
            {
                _intentionalStop = true;
            }
        }

        if (loopRunning)
        {
            return;
        }

        try
        {
            using var stopLinked = CancellationTokenSource.CreateLinkedTokenSource(runToken, cancellationToken);
            await connection.StopAsync(stopLinked.Token);
        }
        finally
        {
            lock (_gate)
            {
                _intentionalStop = false;
            }
        }

        using var connectLinked = CancellationTokenSource.CreateLinkedTokenSource(runToken, cancellationToken);
        var outcome = await TryConnectAsync(connectLinked.Token);
        if (outcome == TaskHubConnectOutcome.Connected)
        {
            ResetReconnectAttempt();
            Log.HubReconnected(_logger);
            Emit(TaskHubEvent.Reconnected);
        }
        else if (outcome == TaskHubConnectOutcome.Unauthorized)
        {
            SetShouldBeConnected(false);
            Emit(TaskHubEvent.Unauthorized);
        }
        else if (outcome == TaskHubConnectOutcome.Failed)
        {
            StartReconnectLoop();
        }
    }

    public IReadOnlyList<TaskHubEvent> DrainBufferedUpdates()
    {
        lock (_gate)
        {
            _buffering = false;
            if (_buffer.Count == 0)
            {
                return [];
            }

            var events = _buffer.ToArray();
            _buffer.Clear();
            return events;
        }
    }

    public async ValueTask DisposeAsync()
    {
        ITaskHubConnection? connection;
        CancellationTokenSource? runCts;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _shouldBeConnected = false;
            _intentionalStop = true;
            connection = _connection;
            runCts = _runCts;
            _connection = null;
            _runCts = null;
            _reconnectWake?.TrySetResult();
        }

        if (runCts is not null)
        {
            await runCts.CancelAsync();
            runCts.Dispose();
        }

        if (connection is not null)
        {
            await connection.DisposeAsync();
        }

        _lifetimeCts.Dispose();
    }

    private static Task DefaultDelayAsync(TimeSpan delay, CancellationToken cancellationToken)
    {
        return Task.Delay(delay, cancellationToken);
    }

    private static bool TryGetStatusCode(Exception exception, out HttpStatusCode statusCode)
    {
        var current = exception;
        while (true)
        {
            if (current is HttpRequestException { StatusCode: { } code })
            {
                statusCode = code;
                return true;
            }

            if (current.InnerException is null)
            {
                statusCode = default;
                return false;
            }

            current = current.InnerException;
        }
    }

    private static bool IsUnauthorized(Exception exception)
    {
        return TryGetStatusCode(exception, out var statusCode) && statusCode == HttpStatusCode.Unauthorized;
    }

    // A secret-safe failure description: exception type and HTTP status only, never a message or URL,
    // because SignalR can place the access token in a negotiate URL that an exception message may echo.
    private static string DescribeFailure(Exception exception)
    {
        return TryGetStatusCode(exception, out var statusCode)
            ? $"{exception.GetType().Name} status {(int)statusCode}"
            : exception.GetType().Name;
    }

    private static string DescribeClose(Exception? error)
    {
        return error is null ? "no error" : DescribeFailure(error);
    }

    private async Task<TaskHubConnectOutcome> TryConnectAsync(CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            _buffering = true;
        }

        try
        {
            await _connection!.StartAsync(cancellationToken);
            return TaskHubConnectOutcome.Connected;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return TaskHubConnectOutcome.Cancelled;
        }
        catch (Exception exception) when (IsUnauthorized(exception))
        {
            Log.HubUnauthorized(_logger);
            return TaskHubConnectOutcome.Unauthorized;
        }
#pragma warning disable CA1031
        catch (Exception exception)
#pragma warning restore CA1031
        {
            Log.HubConnectAttemptFailed(_logger, DescribeFailure(exception));
            return TaskHubConnectOutcome.Failed;
        }
    }

    private void StartReconnectLoop()
    {
        CancellationToken runToken;
        lock (_gate)
        {
            if (_reconnectLoopRunning || !_shouldBeConnected || _runCts is null)
            {
                return;
            }

            _reconnectLoopRunning = true;
            runToken = _runCts.Token;
        }

        _ = RunReconnectLoopAsync(runToken);
    }

    private async Task RunReconnectLoopAsync(CancellationToken runToken)
    {
        try
        {
            while (true)
            {
                int attempt;
                lock (_gate)
                {
                    if (!_shouldBeConnected || runToken.IsCancellationRequested)
                    {
                        return;
                    }

                    attempt = _reconnectAttempt;
                    _buffering = true;
                }

                await WaitBeforeReconnectAsync(TerminalRetryPolicy.GetDelay(attempt), runToken);
                if (runToken.IsCancellationRequested)
                {
                    return;
                }

                lock (_gate)
                {
                    if (!_shouldBeConnected)
                    {
                        return;
                    }
                }

                var outcome = await TryConnectAsync(runToken);
                if (outcome == TaskHubConnectOutcome.Connected)
                {
                    ResetReconnectAttempt();
                    Log.HubReconnected(_logger);
                    Emit(TaskHubEvent.Reconnected);
                    return;
                }

                if (outcome == TaskHubConnectOutcome.Unauthorized)
                {
                    SetShouldBeConnected(false);
                    Emit(TaskHubEvent.Unauthorized);
                    return;
                }

                if (outcome == TaskHubConnectOutcome.Cancelled)
                {
                    return;
                }

                IncrementReconnectAttempt();
            }
        }
        finally
        {
            lock (_gate)
            {
                _reconnectLoopRunning = false;
            }
        }
    }

    // Wait out the backoff delay, but return early if a forced reconnect wakes us or a stop cancels the
    // run. The delay timer is a local linked source (disposed here), and the wake is a plain completion
    // source needing no disposal, so neither can race a disposal from stop, reconnect, or shutdown.
    private async Task WaitBeforeReconnectAsync(TimeSpan delay, CancellationToken runToken)
    {
        var wake = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        lock (_gate)
        {
            _reconnectWake = wake;
        }

        using var delayTimerCts = CancellationTokenSource.CreateLinkedTokenSource(runToken);
        var delayTask = _delayAsync(delay, delayTimerCts.Token);
        var completed = await Task.WhenAny(delayTask, wake.Task);

        lock (_gate)
        {
            _reconnectWake = null;
        }

        if (completed != delayTask)
        {
            await delayTimerCts.CancelAsync();
        }

        try
        {
            await delayTask;
        }
        catch (OperationCanceledException)
        {
            // The delay was cancelled by a stop (runToken) or a forced reconnect; the loop re-checks
            // runToken next and either exits or connects immediately.
        }
    }

    private void HandleUpdate(IReadOnlyList<TaskDto> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        var mapped = tasks.Select(TaskTransportMapper.ToTerminalTask).ToList();
        var hubEvent = TaskHubEvent.Update(mapped);

        bool buffered;
        lock (_gate)
        {
            buffered = _buffering;
            if (buffered)
            {
                _buffer.Add(hubEvent);
            }
        }

        if (!buffered)
        {
            Emit(hubEvent);
        }
    }

    private void HandleHeartbeat()
    {
        Emit(TaskHubEvent.Heartbeat);
    }

    private Task HandleConnectionClosedAsync(Exception? error)
    {
        lock (_gate)
        {
            if (_intentionalStop || !_shouldBeConnected)
            {
                return Task.CompletedTask;
            }

            _buffering = true;
        }

        Log.HubConnectionLost(_logger, DescribeClose(error));
        Emit(TaskHubEvent.Reconnecting);
        StartReconnectLoop();
        return Task.CompletedTask;
    }

    private void Emit(TaskHubEvent hubEvent)
    {
        _eventSink(hubEvent);
    }

    private void SetShouldBeConnected(bool value)
    {
        lock (_gate)
        {
            _shouldBeConnected = value;
        }
    }

    private void ResetReconnectAttempt()
    {
        lock (_gate)
        {
            _reconnectAttempt = 0;
        }
    }

    private void IncrementReconnectAttempt()
    {
        lock (_gate)
        {
            _reconnectAttempt++;
        }
    }
}
