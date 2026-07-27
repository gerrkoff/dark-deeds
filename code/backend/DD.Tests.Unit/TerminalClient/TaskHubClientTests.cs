using System.Diagnostics;
using System.Net;
using DD.Shared.Details.Abstractions.Dto;
using DD.TerminalClient.Details.Realtime;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Realtime;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Coverage of the real-time hub client against the exact server contract, behind a fake connection so no
// socket is opened: the hub URL/query/event names, the reconnect backoff sequence, translation of every
// connectivity condition into domain events, ordered buffering across a snapshot load, stop cancellation,
// and fresh token reads on each connect. All waits use explicit timeouts so a stuck client cannot hang.
public sealed class TaskHubClientTests
{
    [Fact]
    public void TaskHubProtocol_ExposesExactHubPathQueryAndEventNames()
    {
        Assert.Equal("ws/task/task", TaskHubProtocol.HubPath);
        Assert.Equal("clientId", TaskHubProtocol.ClientIdQueryParameter);
        Assert.Equal("update", TaskHubProtocol.UpdateEventName);
        Assert.Equal("heartbeat", TaskHubProtocol.HeartbeatEventName);
    }

    [Fact]
    public void BuildHubUrl_AppendsHubPathAndEscapedClientId_WithSingleTrailingSlash()
    {
        var withSlash = TaskHubProtocol.BuildHubUrl("https://test.dark-deeds.com/", "abc 123");
        Assert.Equal("https://test.dark-deeds.com/ws/task/task?clientId=abc%20123", withSlash.AbsoluteUri);

        var withoutSlash = TaskHubProtocol.BuildHubUrl("http://localhost:5000", "id-1");
        Assert.Equal("http://localhost:5000/ws/task/task?clientId=id-1", withoutSlash.AbsoluteUri);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 4)]
    [InlineData(3, 8)]
    [InlineData(4, 16)]
    [InlineData(5, 30)]
    [InlineData(6, 30)]
    [InlineData(25, 30)]
    public void RetryPolicy_GetDelay_FollowsBackoffThenThirtySecondCeiling(int attempt, int expectedSeconds)
    {
        Assert.Equal(TimeSpan.FromSeconds(expectedSeconds), TerminalRetryPolicy.GetDelay(attempt));
    }

    [Fact]
    public void RetryPolicy_GetDelay_RejectsNegativeAttempt()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TerminalRetryPolicy.GetDelay(-1));
    }

    [Fact]
    public async Task StartAsync_FirstConnectSucceeds_ReadsTokenAndEmitsNoEvent()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory();
        await using var client = new TaskHubClient(
            factory, () => "jwt-1", collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);

        await client.StartAsync(CancellationToken.None);

        Assert.Equal(1, factory.Connection.StartCount);
        Assert.Equal("jwt-1", Assert.Single(factory.Connection.TokensSeen));

        // The first connect is silent - startup loads the initial snapshot itself - so no reconnect fires.
        Assert.Empty(collector.Snapshot());
    }

    [Fact]
    public async Task StartAsync_Unauthorized_EmitsUnauthorizedAndDoesNotRetry()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory(_ => Unauthorized());
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);

        await client.StartAsync(CancellationToken.None);

        Assert.Equal(TaskHubEventKind.Unauthorized, Assert.Single(collector.Kinds()));
        Assert.Equal(1, factory.Connection.StartCount);
    }

    [Fact]
    public async Task StartAsync_CallerCancellation_DoesNotConnectOrEmit()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory();
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await client.StartAsync(cancellation.Token);

        Assert.Empty(collector.Snapshot());
        Assert.Equal(0, factory.Connection.StartCount);
    }

    [Fact]
    public async Task OfflineStart_SchedulesReconnect_AndReconnectsWhenAvailable()
    {
        var collector = new EventCollector();
        var delay = new RecordingDelay();
        var factory = new FakeHubConnectionFactory(index => index == 0 ? Transient() : null);
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, delay.DelayAsync);

        await client.StartAsync(CancellationToken.None);
        await WaitUntilAsync(() => collector.Count(TaskHubEventKind.Reconnected) == 1, "reconnected after offline start");

        Assert.Equal(2, factory.Connection.StartCount);
        Assert.Equal(TimeSpan.FromSeconds(1), delay.Snapshot()[0]);

        // An offline start emits no Reconnecting (nothing was ever connected to lose).
        Assert.DoesNotContain(TaskHubEventKind.Reconnecting, collector.Kinds());
    }

    [Fact]
    public async Task Reconnect_RequestsExpectedBackoffSequenceAcrossConsecutiveFailures()
    {
        var collector = new EventCollector();
        var delay = new RecordingDelay();

        // Initial connect succeeds; reconnect attempts 1..7 fail, attempt 8 succeeds.
        var factory = new FakeHubConnectionFactory(index => index is >= 1 and <= 7 ? Transient() : null);
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, delay.DelayAsync);

        await client.StartAsync(CancellationToken.None);
        await factory.Connection.RaiseClosedAsync();
        await WaitUntilAsync(() => collector.Count(TaskHubEventKind.Reconnected) == 1, "reconnected after backoff");

        var delays = delay.Snapshot();
        Assert.Equal(
            new[] { 1, 2, 4, 8, 16, 30, 30, 30 },
            delays.Take(8).Select(d => (int)d.TotalSeconds).ToArray());
        Assert.Equal(9, factory.Connection.StartCount);
    }

    [Fact]
    public async Task Update_IsTranslatedToDomainEventWithMappedTasks()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory();
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);

        await client.StartAsync(CancellationToken.None);

        // Finish the initial buffering window so a subsequent push flows straight to the sink.
        client.DrainBufferedUpdates();
        factory.Connection.RaiseUpdate([Dto("u1", version: 3, type: TaskTypeDto.Routine)]);

        var update = Assert.Single(collector.Snapshot(), e => e.Kind == TaskHubEventKind.Update);
        var task = Assert.Single(update.Tasks);
        Assert.Equal("u1", task.Uid);
        Assert.Equal(3, task.Version);
        Assert.Equal(TerminalTaskType.Routine, task.Type);
    }

    [Fact]
    public async Task Heartbeat_IsTranslatedToHeartbeatEvent()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory();
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);

        await client.StartAsync(CancellationToken.None);
        factory.Connection.RaiseHeartbeat();

        Assert.Contains(TaskHubEventKind.Heartbeat, collector.Kinds());
    }

    [Fact]
    public async Task ConnectionDropWhileConnected_EmitsReconnectingThenReconnected()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory();
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);

        await client.StartAsync(CancellationToken.None);
        await factory.Connection.RaiseClosedAsync(new HttpRequestException("dropped"));
        await WaitUntilAsync(() => collector.Count(TaskHubEventKind.Reconnected) == 1, "reconnected after drop");

        var kinds = collector.Kinds();
        var reconnectingIndex = Array.IndexOf(kinds, TaskHubEventKind.Reconnecting);
        var reconnectedIndex = Array.IndexOf(kinds, TaskHubEventKind.Reconnected);
        Assert.True(reconnectingIndex >= 0);
        Assert.True(reconnectedIndex > reconnectingIndex);
    }

    [Fact]
    public async Task UnauthorizedDuringReconnect_EmitsUnauthorizedAndStopsRetrying()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory(index => index == 0 ? null : Unauthorized());
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);

        await client.StartAsync(CancellationToken.None);
        await factory.Connection.RaiseClosedAsync();
        await WaitUntilAsync(() => collector.Count(TaskHubEventKind.Unauthorized) == 1, "unauthorized during reconnect");

        var attemptsAfterUnauthorized = factory.Connection.StartCount;
        await Task.Delay(50);
        Assert.Equal(attemptsAfterUnauthorized, factory.Connection.StartCount);
        Assert.Contains(TaskHubEventKind.Reconnecting, collector.Kinds());
    }

    [Fact]
    public async Task StopAsync_EmitsClosedAndSuppressesFurtherReconnect()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory();
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);

        await client.StartAsync(CancellationToken.None);
        await client.StopAsync(CancellationToken.None);

        Assert.Contains(TaskHubEventKind.Closed, collector.Kinds());
        Assert.True(factory.Connection.StopCount >= 1);

        // A close callback after an intentional stop must not restart the connection.
        await factory.Connection.RaiseClosedAsync();
        Assert.Equal(1, factory.Connection.StartCount);
        Assert.DoesNotContain(TaskHubEventKind.Reconnecting, collector.Kinds());
    }

    [Fact]
    public async Task StartAsync_AfterStop_ReconnectsWithAFreshConnection()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory();
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);

        await client.StartAsync(CancellationToken.None);
        var firstConnection = factory.Connection;
        await client.StopAsync(CancellationToken.None);

        await client.StartAsync(CancellationToken.None);

        // A re-login after a 401 must restart the hub: Stop resets the client so Start builds and connects
        // a fresh connection instead of returning early and leaving the client permanently disconnected.
        Assert.NotSame(firstConnection, factory.Connection);
        Assert.Equal(1, factory.Connection.StartCount);
        Assert.True(firstConnection.DisposeCount >= 1);
    }

    [Fact]
    public async Task StopAsync_CancelsAPendingReconnectWait()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory();
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, InfiniteDelayAsync);

        await client.StartAsync(CancellationToken.None);
        await factory.Connection.RaiseClosedAsync();
        await WaitUntilAsync(() => collector.Count(TaskHubEventKind.Reconnecting) == 1, "reconnect loop waiting");

        // The loop is blocked in its backoff wait; stopping must cancel it before another attempt.
        await client.StopAsync(CancellationToken.None);
        await WaitUntilAsync(() => collector.Count(TaskHubEventKind.Closed) == 1, "closed");

        Assert.Equal(1, factory.Connection.StartCount);
    }

    [Fact]
    public async Task Buffering_HoldsUpdatesInArrivalOrder_UntilDrained()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory();
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);

        await client.StartAsync(CancellationToken.None);
        factory.Connection.RaiseUpdate([Dto("a")]);
        factory.Connection.RaiseUpdate([Dto("b")]);

        Assert.DoesNotContain(collector.Snapshot(), e => e.Kind == TaskHubEventKind.Update);

        var drained = client.DrainBufferedUpdates();
        Assert.Equal(2, drained.Count);
        Assert.Equal("a", Assert.Single(drained[0].Tasks).Uid);
        Assert.Equal("b", Assert.Single(drained[1].Tasks).Uid);

        // After draining, buffering is closed and the next push is delivered live.
        factory.Connection.RaiseUpdate([Dto("c")]);
        var live = Assert.Single(collector.Snapshot(), e => e.Kind == TaskHubEventKind.Update);
        Assert.Equal("c", Assert.Single(live.Tasks).Uid);
    }

    [Fact]
    public async Task Buffering_ReopensOnReconnect_SoPushesWaitForTheNextSnapshot()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory();
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);

        await client.StartAsync(CancellationToken.None);
        client.DrainBufferedUpdates();
        factory.Connection.RaiseUpdate([Dto("live")]);
        Assert.Contains(collector.Snapshot(), e => e.Kind == TaskHubEventKind.Update && e.Tasks[0].Uid == "live");

        await factory.Connection.RaiseClosedAsync();
        await WaitUntilAsync(() => collector.Count(TaskHubEventKind.Reconnected) == 1, "reconnected");

        factory.Connection.RaiseUpdate([Dto("after-reconnect")]);
        Assert.DoesNotContain(
            collector.Snapshot(),
            e => e.Kind == TaskHubEventKind.Update && e.Tasks[0].Uid == "after-reconnect");

        var drained = client.DrainBufferedUpdates();
        Assert.Contains(drained, e => e.Tasks[0].Uid == "after-reconnect");
    }

    [Fact]
    public async Task Reconnect_ReadsAFreshTokenOnEachConnect()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory();
        var token = "jwt-1";
        await using var client = new TaskHubClient(
            factory, () => token, collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);

        await client.StartAsync(CancellationToken.None);
        token = "jwt-2";
        await factory.Connection.RaiseClosedAsync();
        await WaitUntilAsync(() => collector.Count(TaskHubEventKind.Reconnected) == 1, "reconnected");

        Assert.Equal(new[] { "jwt-1", "jwt-2" }, factory.Connection.TokensSeen);
    }

    [Fact]
    public async Task ReconnectNow_WhenConnected_DropsAndReconnects()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory();
        await using var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);

        await client.StartAsync(CancellationToken.None);
        await client.ReconnectNowAsync(CancellationToken.None);

        Assert.Contains(TaskHubEventKind.Reconnected, collector.Kinds());
        Assert.Equal(2, factory.Connection.StartCount);
        Assert.True(factory.Connection.StopCount >= 1);
    }

    [Fact]
    public async Task DisposeAsync_DisposesConnectionAndBlocksFurtherStart()
    {
        var collector = new EventCollector();
        var factory = new FakeHubConnectionFactory();
        var client = new TaskHubClient(
            factory, () => "jwt", collector.Add, NullLogger<TaskHubClient>.Instance, ImmediateDelayAsync);

        await client.StartAsync(CancellationToken.None);
        await client.DisposeAsync();

        Assert.Equal(1, factory.Connection.DisposeCount);
        await Assert.ThrowsAsync<ObjectDisposedException>(() => client.StartAsync(CancellationToken.None));
    }

    private static Task ImmediateDelayAsync(TimeSpan delay, CancellationToken cancellationToken)
    {
        return cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) : Task.CompletedTask;
    }

    private static Task InfiniteDelayAsync(TimeSpan delay, CancellationToken cancellationToken)
    {
        return Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    private static HttpRequestException Unauthorized()
    {
        return new HttpRequestException("Unauthorized", null, HttpStatusCode.Unauthorized);
    }

    private static HttpRequestException Transient()
    {
        return new HttpRequestException("The remote host refused the connection.");
    }

    private static TaskDto Dto(string uid, int version = 1, TaskTypeDto type = TaskTypeDto.Simple)
    {
        return new TaskDto { Uid = uid, Title = uid, Version = version, Type = type };
    }

    private static async Task WaitUntilAsync(Func<bool> condition, string because)
    {
        var timeout = TimeSpan.FromSeconds(5);
        var stopwatch = Stopwatch.StartNew();
        while (!condition())
        {
            if (stopwatch.Elapsed > timeout)
            {
                throw new TimeoutException($"Timed out waiting for {because}.");
            }

            await Task.Delay(10);
        }
    }

    private sealed class EventCollector
    {
        private readonly object _sync = new();
        private readonly List<TaskHubEvent> _events = [];

        public void Add(TaskHubEvent hubEvent)
        {
            lock (_sync)
            {
                _events.Add(hubEvent);
            }
        }

        public TaskHubEvent[] Snapshot()
        {
            lock (_sync)
            {
                return [.. _events];
            }
        }

        public TaskHubEventKind[] Kinds()
        {
            lock (_sync)
            {
                return _events.Select(e => e.Kind).ToArray();
            }
        }

        public int Count(TaskHubEventKind kind)
        {
            lock (_sync)
            {
                return _events.Count(e => e.Kind == kind);
            }
        }
    }

    private sealed class RecordingDelay
    {
        private readonly object _sync = new();
        private readonly List<TimeSpan> _delays = [];

        public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken)
        {
            lock (_sync)
            {
                _delays.Add(delay);
            }

            return cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) : Task.CompletedTask;
        }

        public TimeSpan[] Snapshot()
        {
            lock (_sync)
            {
                return [.. _delays];
            }
        }
    }

    private sealed class FakeHubConnectionFactory(Func<int, Exception?>? startBehavior = null) : ITaskHubConnectionFactory
    {
        public FakeHubConnection Connection { get; private set; } = null!;

        public ITaskHubConnection Create(Func<string?> tokenProvider)
        {
            Connection = new FakeHubConnection(tokenProvider, startBehavior);
            return Connection;
        }
    }

    private sealed class FakeHubConnection(Func<string?> tokenProvider, Func<int, Exception?>? startBehavior)
        : ITaskHubConnection
    {
        private readonly object _sync = new();
        private Action<IReadOnlyList<TaskDto>>? _onUpdate;
        private Action? _onHeartbeat;
        private Func<Exception?, Task>? _onClosed;

        public List<string?> TokensSeen { get; } = [];

        public int StartCount { get; private set; }

        public int StopCount { get; private set; }

        public int DisposeCount { get; private set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int index;
            lock (_sync)
            {
                index = StartCount;
                StartCount++;
                TokensSeen.Add(tokenProvider());
            }

            var failure = startBehavior?.Invoke(index);
            if (failure is not null)
            {
                throw failure;
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            lock (_sync)
            {
                StopCount++;
            }

            return Task.CompletedTask;
        }

        public void OnUpdate(Action<IReadOnlyList<TaskDto>> handler)
        {
            _onUpdate = handler;
        }

        public void OnHeartbeat(Action handler)
        {
            _onHeartbeat = handler;
        }

        public void OnClosed(Func<Exception?, Task> handler)
        {
            _onClosed = handler;
        }

        public ValueTask DisposeAsync()
        {
            lock (_sync)
            {
                DisposeCount++;
            }

            return ValueTask.CompletedTask;
        }

        public void RaiseUpdate(IReadOnlyList<TaskDto> tasks)
        {
            _onUpdate!(tasks);
        }

        public void RaiseHeartbeat()
        {
            _onHeartbeat!();
        }

        public Task RaiseClosedAsync(Exception? error = null)
        {
            return _onClosed!(error);
        }
    }
}
