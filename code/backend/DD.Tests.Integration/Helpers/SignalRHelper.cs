using DD.Shared.Details.Abstractions.Dto;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace DD.Tests.Integration.Helpers;

internal sealed class SignalRUpdateCollector : IAsyncDisposable
{
    private readonly HubConnection _connection;
    private readonly object _sync = new();
    private readonly Dictionary<string, TaskDto> _tasksByUid = [];
    private readonly List<string> _arrivalOrder = [];
    private readonly HashSet<string> _arrivalUids = [];
    private readonly List<Waiter> _waiters = [];

    private SignalRUpdateCollector(HubConnection connection)
    {
        _connection = connection;
        _connection.On<List<TaskDto>>("update", RecordUpdate);
    }

    public IReadOnlyList<string> ArrivalOrder
    {
        get
        {
            lock (_sync)
            {
                return [.. _arrivalOrder];
            }
        }
    }

    public IReadOnlyList<string> ReceivedTaskUids
    {
        get
        {
            lock (_sync)
            {
                return [.. _tasksByUid.Keys];
            }
        }
    }

    public static async Task<SignalRUpdateCollector> ConnectAsync(
        HttpMessageHandler handler,
        string token,
        string? clientId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var hubUrl = new Uri(
            $"http://localhost/ws/task/task{BuildClientIdQuery(clientId)}",
            UriKind.Absolute);
        var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                options.HttpMessageHandlerFactory = _ => handler;
                options.Transports = HttpTransportType.LongPolling;
            })
            .Build();

        try
        {
            var collector = new SignalRUpdateCollector(connection);
            await connection.StartAsync(cancellationToken);
            return collector;
        }
        catch
        {
            await using (connection.ConfigureAwait(false))
            {
            }

            throw;
        }
    }

    public async Task<TaskDto> WaitForTaskAsync(
        Func<TaskDto, bool> predicate,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var waiter = new Waiter(predicate);
        lock (_sync)
        {
            var existing = _tasksByUid.Values.FirstOrDefault(predicate);
            if (existing is not null)
                return existing;

            _waiters.Add(waiter);
        }

        try
        {
            return await waiter.Completion.Task.WaitAsync(timeout, cancellationToken);
        }
        finally
        {
            lock (_sync)
            {
                _waiters.Remove(waiter);
            }
        }
    }

    public Task<TaskDto> WaitForTaskAsync(
        string uid,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        return WaitForTaskAsync(task => task.Uid == uid, timeout, cancellationToken);
    }

    public bool HasReceived(string uid)
    {
        lock (_sync)
        {
            return _tasksByUid.ContainsKey(uid);
        }
    }

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }

    private static string BuildClientIdQuery(string? clientId)
    {
        return string.IsNullOrWhiteSpace(clientId)
            ? string.Empty
            : $"?clientId={Uri.EscapeDataString(clientId)}";
    }

    private void RecordUpdate(IReadOnlyList<TaskDto> tasks)
    {
        List<(Waiter Waiter, TaskDto Task)> completions = [];
        lock (_sync)
        {
            foreach (var task in tasks)
            {
                _tasksByUid[task.Uid] = task;
                if (_arrivalUids.Add(task.Uid))
                    _arrivalOrder.Add(task.Uid);

                foreach (var waiter in _waiters)
                {
                    if (waiter.Predicate(task))
                        completions.Add((waiter, task));
                }
            }

            foreach (var (waiter, _) in completions)
                _waiters.Remove(waiter);
        }

        foreach (var (waiter, task) in completions)
            waiter.Completion.TrySetResult(task);
    }

    private sealed class Waiter(Func<TaskDto, bool> predicate)
    {
        public Func<TaskDto, bool> Predicate { get; } = predicate;

        public TaskCompletionSource<TaskDto> Completion { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
