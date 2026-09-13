using DD.Shared.Details.Abstractions.Dto;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace DD.Tests.Integration.Infrastructure.Clients;

internal sealed class TestSignalRClient : IAsyncDisposable
{
    private readonly HubConnection _connection;
    private readonly HttpMessageHandler _handler;
    private readonly object _sync = new();
    private readonly Dictionary<string, TaskDto> _tasksByUid = [];
    private readonly List<string> _arrivalOrder = [];
    private readonly List<Waiter> _waiters = [];

    private TestSignalRClient(HubConnection connection, HttpMessageHandler handler)
    {
        _connection = connection;
        _handler = handler;
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

    public static async Task<TestSignalRClient> CreateAsync(
        TestUserClient user,
        string? clientId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var handler = await IntegrationEnvironmentLifetime.CreateSignalRHandlerAsync();
        var hubUrl = new Uri(
            $"http://localhost/ws/task/task{BuildClientIdQuery(clientId)}",
            UriKind.Absolute);
        var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(user.Token);
                options.HttpMessageHandlerFactory = _ => handler;
                options.Transports = HttpTransportType.LongPolling;
            })
            .Build();

        try
        {
            var client = new TestSignalRClient(connection, handler);
            await connection.StartAsync(cancellationToken);
            return client;
        }
        catch
        {
            await connection.DisposeAsync().ConfigureAwait(false);
            handler.Dispose();
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

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _connection.DisposeAsync();
        }
        finally
        {
            _handler.Dispose();
        }
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
                if (!_tasksByUid.ContainsKey(task.Uid))
                    _arrivalOrder.Add(task.Uid);

                _tasksByUid[task.Uid] = task;

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
