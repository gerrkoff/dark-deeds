using System.Text;
using DD.Shared.TaskText;
using DD.TerminalClient;
using DD.TerminalClient.Details.Ui;
using DD.TerminalClient.Domain.Abstractions;
using DD.TerminalClient.Domain.Application;
using DD.TerminalClient.Domain.Authentication;
using DD.TerminalClient.Domain.Editing;
using DD.TerminalClient.Domain.Input;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Overview;
using DD.TerminalClient.Domain.Realtime;
using DD.TerminalClient.Domain.State;
using DD.TerminalClient.Domain.Time;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Drives the whole event loop against fake console/clock/storage/API/hub adapters, exercising the
// startup/reconnect/401 sequence, offline cached startup, the same/different-user data-owner guard,
// token renewal, save retry, conflict notification, resize gating, quit, and fatal cleanup. Every wait
// carries an explicit timeout and the wall-clock timers are switched off, so a run can never hang.
public sealed class TerminalApplicationTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);
    private static readonly DateTimeOffset Now = new(2024, 6, 10, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Startup_ValidToken_LoadsSnapshotAndShowsTasks()
    {
        using var harness = new Harness();
        harness.TokenStore.Save(Jwt("alice", Now.AddDays(30)));
        harness.StateStore.Set(new PersistedTerminalState { DataOwner = "alice" });
        harness.Tasks.LoadResult = [Task("srv", "From server")];

        var run = harness.Start();
        await WaitForAsync(() => HasTask(harness.LastModel(), "srv"), "server task rendered");

        Assert.True(harness.Hub.StartCount >= 1);
        Assert.False(harness.LastModel().IsOffline);
        await harness.StopAsync(run);
    }

    [Fact]
    public async Task Startup_LoadFails_ShowsCachedTasksOffline()
    {
        using var harness = new Harness();
        harness.TokenStore.Save(Jwt("alice", Now.AddDays(30)));
        harness.StateStore.Set(new PersistedTerminalState
        {
            DataOwner = "alice",
            CachedTasks = [Task("cached", "Cached task")],
        });
        harness.Tasks.LoadHandler = () => throw new TerminalApiException(TerminalApiErrorKind.Transport, "down");

        var run = harness.Start();
        await WaitForAsync(() => harness.LastModel().IsOffline, "offline indicator");

        Assert.True(HasTask(harness.LastModel(), "cached"));
        await harness.StopAsync(run);
    }

    [Fact]
    public async Task Login_ValidCredentials_ProceedsToReady()
    {
        using var harness = new Harness();
        harness.Auth.SignInHandler = (user, _) =>
            new SignInOutcome(TerminalSignInStatus.Success, AuthSession.FromToken(Jwt(user, Now.AddDays(30))));

        var run = harness.Start();
        await WaitForAsync(() => harness.LastModel().Status.Kind == TerminalStatusKind.Login, "login prompt");

        harness.TypeLine("bob");
        harness.TypeLine("secret");
        await WaitForAsync(() => harness.LastModel().Status.Kind == TerminalStatusKind.Normal, "ready");

        Assert.Equal(("bob", "secret"), Assert.Single(harness.Auth.SignInCalls));
        Assert.NotNull(harness.CurrentToken);
        await harness.StopAsync(run);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ShowsErrorAndStaysLogin()
    {
        using var harness = new Harness();
        harness.Auth.SignInHandler = (_, _) => new SignInOutcome(TerminalSignInStatus.InvalidCredentials, null);

        var run = harness.Start();
        await WaitForAsync(() => harness.LastModel().Status.Kind == TerminalStatusKind.Login, "login prompt");

        harness.TypeLine("bob");
        harness.TypeLine("wrong");
        await WaitForAsync(
            () => harness.LastModel().Notification == "Invalid username or password.", "invalid-credentials notice");

        Assert.Equal(TerminalStatusKind.Login, harness.LastModel().Status.Kind);
        await harness.StopAsync(run);
    }

    [Fact]
    public async Task Startup_SameUser_DrainsPersistedOutbox()
    {
        using var harness = new Harness();
        harness.TokenStore.Save(Jwt("alice", Now.AddDays(30)));
        harness.StateStore.Set(new PersistedTerminalState
        {
            DataOwner = "alice",
            CachedTasks = [Task("a", "Edited", version: 2)],
            Outbox = [Task("a", "Edited", version: 2)],
        });

        var run = harness.Start();
        await WaitForAsync(() => harness.Tasks.SavedBatches.Count >= 1, "outbox drained");

        Assert.Equal("a", Assert.Single(harness.Tasks.SavedBatches[0]).Uid);
        await harness.StopAsync(run);
    }

    [Fact]
    public async Task Startup_DifferentUser_PromptsThenClearsOnConfirm()
    {
        using var harness = new Harness();
        harness.TokenStore.Save(Jwt("bob", Now.AddDays(30)));
        harness.StateStore.Set(new PersistedTerminalState
        {
            DataOwner = "alice",
            CachedTasks = [Task("old", "Alice task")],
        });

        var run = harness.Start();
        await WaitForAsync(
            () => harness.LastModel().Status.Kind == TerminalStatusKind.Confirmation, "reset confirmation");

        harness.Enqueue(Key('y'));
        await WaitForAsync(() => harness.StateStore.Saved?.DataOwner == "bob", "owner updated to bob");

        Assert.Empty(harness.StateStore.Saved!.CachedTasks);
        Assert.False(HasTask(harness.LastModel(), "old"));
        await harness.StopAsync(run);
    }

    [Fact]
    public async Task HubUnauthorized_ReturnsToLoginAndStopsHub()
    {
        using var harness = new Harness();
        harness.TokenStore.Save(Jwt("alice", Now.AddDays(30)));
        harness.StateStore.Set(new PersistedTerminalState { DataOwner = "alice" });

        var run = harness.Start();
        await WaitForAsync(() => harness.Hub.StartCount >= 1, "hub started");

        harness.Hub.Raise(TaskHubEvent.Unauthorized);
        await WaitForAsync(
            () => harness.LastModel().Status.Kind == TerminalStatusKind.Login, "returned to login");

        Assert.True(harness.Hub.StopCount >= 1);
        Assert.Null(harness.CurrentToken);
        await harness.StopAsync(run);
    }

    [Fact]
    public async Task Renew_WhenExpiringWithinOneDay_RenewsAndPersistsToken()
    {
        using var harness = new Harness();
        harness.TokenStore.Save(Jwt("alice", Now.AddHours(6)));
        harness.StateStore.Set(new PersistedTerminalState { DataOwner = "alice" });
        var renewed = Jwt("alice", Now.AddDays(30));
        harness.Auth.RenewHandler = () => AuthSession.FromToken(renewed);

        var run = harness.Start();
        await WaitForAsync(() => harness.Hub.StartCount >= 1, "hub started");

        harness.Enqueue(ApplicationEvent.RenewTick);
        await WaitForAsync(() => harness.TokenStore.Load() == renewed, "token renewed");

        Assert.Equal(1, harness.Auth.RenewCount);
        Assert.Equal(renewed, harness.CurrentToken);
        await harness.StopAsync(run);
    }

    [Fact]
    public async Task Save_TransportError_SchedulesRetryThenSucceeds()
    {
        using var harness = new Harness(immediateDelay: true);
        harness.TokenStore.Save(Jwt("alice", Now.AddDays(30)));
        harness.StateStore.Set(new PersistedTerminalState
        {
            DataOwner = "alice",
            CachedTasks = [Task("a", "Task A")],
        });
        harness.Tasks.LoadResult = [Task("a", "Task A")];
        var attempts = 0;
        harness.Tasks.SaveHandler = batch =>
        {
            attempts++;
            return attempts == 1
                ? throw new TerminalApiException(TerminalApiErrorKind.Transport, "down")
                : Echo(batch);
        };

        var run = harness.Start();
        await WaitForAsync(() => HasTask(harness.LastModel(), "a"), "task rendered");

        harness.Enqueue(Space());
        await WaitForAsync(() => harness.Tasks.SavedBatches.Count >= 2, "save retried");

        await harness.StopAsync(run);
    }

    [Fact]
    public async Task HubUpdate_NewerVersionThanPendingEdit_NotifiesConflict()
    {
        using var harness = new Harness();
        harness.TokenStore.Save(Jwt("alice", Now.AddDays(30)));
        harness.StateStore.Set(new PersistedTerminalState
        {
            DataOwner = "alice",
            CachedTasks = [Task("a", "Task A", version: 1)],
        });
        harness.Tasks.LoadResult = [Task("a", "Task A", version: 1)];
        harness.Tasks.HoldSaves();

        var run = harness.Start();
        await WaitForAsync(() => HasTask(harness.LastModel(), "a"), "task rendered");

        harness.Enqueue(Space());
        await WaitForAsync(() => harness.Tasks.SavedBatches.Count >= 1, "edit in flight");

        harness.Hub.Raise(TaskHubEvent.Update([Task("a", "Server wins", version: 5)]));
        await WaitForAsync(
            () => harness.LastModel().Notification?.Contains("Server wins", StringComparison.Ordinal) == true,
            "conflict notice");

        await harness.StopAsync(run);
    }

    [Fact]
    public async Task Resize_BelowMinimum_RendersResizeRequired()
    {
        using var harness = new Harness();
        harness.TokenStore.Save(Jwt("alice", Now.AddDays(30)));
        harness.StateStore.Set(new PersistedTerminalState { DataOwner = "alice" });

        var run = harness.Start();
        await WaitForAsync(() => harness.LastModel().Status.Kind == TerminalStatusKind.Normal, "ready");

        harness.Enqueue(ApplicationEvent.Resized(100, 20));
        await WaitForAsync(harness.LastResizeRequired, "resize-required frame");

        harness.Enqueue(ApplicationEvent.Resized(140, 40));
        await WaitForAsync(() => !harness.LastResizeRequired(), "normal frame after growing");

        await harness.StopAsync(run);
    }

    [Fact]
    public async Task QuitKey_StopsLoopAndRestoresScreen()
    {
        using var harness = new Harness();
        harness.TokenStore.Save(Jwt("alice", Now.AddDays(30)));
        harness.StateStore.Set(new PersistedTerminalState { DataOwner = "alice" });

        var run = harness.Start();
        await WaitForAsync(() => harness.LastModel().Status.Kind == TerminalStatusKind.Normal, "ready");

        harness.Enqueue(Key('q'));
        await run.WaitAsync(Timeout);

        Assert.True(harness.Renderer.EndCount >= 1);
    }

    [Fact]
    public async Task RenderThrows_PersistsStateStopsHubAndRestoresScreen()
    {
        using var harness = new Harness();
        harness.Renderer.ThrowAfter = 1;

        var run = harness.Start();
        harness.Enqueue(ApplicationEvent.Resized(130, 40));
        await run.WaitAsync(Timeout);

        Assert.NotNull(harness.App.FatalMessage);
        Assert.True(harness.Renderer.EndCount >= 1);
        Assert.NotNull(harness.StateStore.Saved);
        Assert.True(harness.Hub.DisposeCount >= 1);
    }

    [Fact]
    public async Task HubReconnected_ReloadsSnapshot()
    {
        using var harness = new Harness();
        harness.TokenStore.Save(Jwt("alice", Now.AddDays(30)));
        harness.StateStore.Set(new PersistedTerminalState { DataOwner = "alice" });

        var run = harness.Start();
        await WaitForAsync(() => harness.Tasks.LoadCount >= 1, "initial snapshot");

        harness.Hub.Raise(TaskHubEvent.Reconnected);
        await WaitForAsync(() => harness.Tasks.LoadCount >= 2, "snapshot reloaded on reconnect");

        await harness.StopAsync(run);
    }

    [Fact]
    public async Task Unauthorized_PreservesDurableOutbox()
    {
        using var harness = new Harness();
        harness.TokenStore.Save(Jwt("alice", Now.AddDays(30)));
        harness.StateStore.Set(new PersistedTerminalState
        {
            DataOwner = "alice",
            CachedTasks = [Task("a", "Task A")],
            Outbox = [Task("a", "Task A")],
        });
        harness.Tasks.LoadResult = [Task("a", "Task A")];
        harness.Tasks.HoldSaves();

        var run = harness.Start();
        await WaitForAsync(() => harness.Tasks.SavedBatches.Count >= 1, "outbox drain in flight");

        harness.Hub.Raise(TaskHubEvent.Unauthorized);
        await WaitForAsync(
            () => harness.LastModel().Status.Kind == TerminalStatusKind.Login, "returned to login");

        await harness.StopAsync(run);

        Assert.Equal("a", Assert.Single(harness.StateStore.Saved!.Outbox).Uid);
    }

    [Fact]
    public async Task Unauthorized_CancelsInFlightNetworkActivity()
    {
        using var harness = new Harness();
        harness.TokenStore.Save(Jwt("alice", Now.AddDays(30)));
        harness.StateStore.Set(new PersistedTerminalState
        {
            DataOwner = "alice",
            CachedTasks = [Task("a", "Task A")],
            Outbox = [Task("a", "Task A")],
        });
        harness.Tasks.LoadResult = [Task("a", "Task A")];
        harness.Tasks.HoldSaves();

        var run = harness.Start();
        await WaitForAsync(() => harness.Tasks.SavedBatches.Count >= 1, "outbox drain in flight");
        Assert.False(harness.Tasks.LastSaveToken.IsCancellationRequested);

        harness.Hub.Raise(TaskHubEvent.Unauthorized);
        await WaitForAsync(
            () => harness.Tasks.LastSaveToken.IsCancellationRequested, "in-flight save cancelled on 401");

        await harness.StopAsync(run);
    }

    [Fact]
    public async Task UnreadableState_IsNotOverwrittenAndStopsWithFatalMessage()
    {
        using var harness = new Harness();
        harness.StateStore.Set(new PersistedTerminalState
        {
            DataOwner = "alice",
            CachedTasks = [Task("a", "Task A")],
            Outbox = [Task("a", "Task A")],
        });
        harness.StateStore.LoadFault =
            new TerminalStateException("Local state cannot be loaded because it is not valid JSON.");

        var run = harness.Start();
        await run.WaitAsync(Timeout);

        Assert.NotNull(harness.App.FatalMessage);
        Assert.Null(harness.StateStore.Saved);
    }

    [Fact]
    public async Task ReloadRetry_SurvivesHeartbeatWhileBuffering()
    {
        using var harness = new Harness();
        harness.TokenStore.Save(Jwt("alice", Now.AddDays(30)));
        harness.StateStore.Set(new PersistedTerminalState { DataOwner = "alice" });
        var loads = 0;
        harness.Tasks.LoadHandler = () =>
        {
            loads++;
            if (loads == 1)
            {
                throw new TerminalApiException(TerminalApiErrorKind.Transport, "down");
            }

            return [];
        };

        var run = harness.Start();
        await WaitForAsync(() => harness.LastModel().IsOffline, "offline after failed snapshot");

        harness.Hub.Raise(TaskHubEvent.Heartbeat);
        harness.Enqueue(ApplicationEvent.ReloadSnapshotTick);
        await WaitForAsync(() => harness.Tasks.LoadCount >= 2, "snapshot reloaded despite heartbeat");

        await harness.StopAsync(run);
    }

    private static async Task WaitForAsync(Func<bool> condition, string because)
    {
        var deadline = DateTime.UtcNow + Timeout;
        while (!condition())
        {
            if (DateTime.UtcNow > deadline)
            {
                throw new TimeoutException($"Timed out waiting for {because}.");
            }

            await System.Threading.Tasks.Task.Delay(10);
        }
    }

    private static bool HasTask(TerminalViewModel model, string uid)
    {
        var cells = new List<OverviewDay> { model.Overview.NoDate };
        cells.AddRange(model.Overview.Overdue);
        cells.AddRange(model.Overview.Current);
        cells.AddRange(model.Overview.Future);
        return cells.SelectMany(cell => cell.Tasks)
            .Any(task => string.Equals(task.Task.Uid, uid, StringComparison.Ordinal));
    }

    private static List<TerminalTask> Echo(IReadOnlyCollection<TerminalTask> batch)
    {
        return batch.Select(task => task with { Version = task.Version + 1 }).ToList();
    }

    private static TerminalTask Task(string uid, string title, int version = 0)
    {
        return new TerminalTask { Uid = uid, Title = title, Version = version };
    }

    private static ConsoleKeyInfo Key(char character)
    {
        var key = char.IsLetter(character) ? (ConsoleKey)char.ToUpperInvariant(character) : 0;
        return new ConsoleKeyInfo(character, key, shift: char.IsUpper(character), alt: false, control: false);
    }

    private static ConsoleKeyInfo Space()
    {
        return new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, shift: false, alt: false, control: false);
    }

    private static ConsoleKeyInfo Enter()
    {
        return new ConsoleKeyInfo('\r', ConsoleKey.Enter, shift: false, alt: false, control: false);
    }

    private static string Jwt(string user, DateTimeOffset expiry)
    {
        static string Segment(string json)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json))
                .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        var header = Segment("{\"alg\":\"none\"}");
        var payload = Segment($"{{\"name\":\"{user}\",\"exp\":{expiry.ToUnixTimeSeconds()}}}");
        return $"{header}.{payload}.signature";
    }

    private sealed record Frame(TerminalViewModel Model, bool ResizeRequired);

    private sealed class Harness : IDisposable
    {
        private readonly CancellationTokenSource _cts = new();
        private int _uid;

        public Harness(bool immediateDelay = false)
        {
            var dates = new FixedDates();
            var parser = new TaskTextParser(dates);
            var formatter = new TaskTextFormatter(dates);
            var reducer = new ApplicationReducer(
                new TerminalInputReducer(parser, formatter),
                new OverviewProjectionService(dates),
                parser,
                formatter,
                NextUid);

            var dependencies = new TerminalDependencies
            {
                Auth = Auth,
                Tasks = Tasks,
                HubFactory = sink =>
                {
                    Hub.Sink = sink;
                    return Hub;
                },
                StateStore = StateStore,
                TokenStore = TokenStore,
                Keys = Keys,
                Renderer = Renderer,
                Reducer = reducer,
                LocalDate = dates,
                Clock = new FixedClock(Now),
                SetToken = value => CurrentToken = value,
                ReadDimensions = () => (140, 40),
                ProfileName = "test",
                Delay = immediateDelay
                    ? (_, _) => System.Threading.Tasks.Task.CompletedTask
                    : (delay, cancellationToken) => System.Threading.Tasks.Task.Delay(
                        System.Threading.Timeout.InfiniteTimeSpan, cancellationToken),
                StartBackgroundTimers = false,
            };

            App = new TerminalApplication(dependencies);
        }

        public FakeAuthApi Auth { get; } = new();

        public FakeTaskApi Tasks { get; } = new();

        public FakeHub Hub { get; } = new();

        public InMemoryStateStore StateStore { get; } = new();

        public InMemoryTokenStore TokenStore { get; } = new();

        public BlockingKeySource Keys { get; } = new();

        public CaptureRenderer Renderer { get; } = new();

        public TerminalApplication App { get; }

        public string? CurrentToken { get; private set; }

        public Task Start()
        {
            return App.RunAsync(_cts.Token);
        }

        public void Enqueue(ApplicationEvent applicationEvent)
        {
            App.Enqueue(applicationEvent);
        }

        public void Enqueue(ConsoleKeyInfo key)
        {
            App.Enqueue(ApplicationEvent.KeyPressed(key));
        }

        public void TypeLine(string text)
        {
            foreach (var character in text)
            {
                Enqueue(Key(character));
            }

            Enqueue(Enter());
        }

        public TerminalViewModel LastModel()
        {
            return Renderer.Last()?.Model
                ?? throw new InvalidOperationException("Nothing has been rendered yet.");
        }

        public bool LastResizeRequired()
        {
            return Renderer.Last()?.ResizeRequired ?? false;
        }

        public async Task StopAsync(Task run)
        {
            await _cts.CancelAsync();
            await run.WaitAsync(Timeout);
        }

        public void Dispose()
        {
            _cts.Dispose();
        }

        private string NextUid()
        {
            return $"new-{++_uid}";
        }
    }

    private sealed class FakeAuthApi : IAuthApiClient
    {
        private readonly object _gate = new();

        public Func<string, string, SignInOutcome> SignInHandler { get; set; } =
            (_, _) => new SignInOutcome(TerminalSignInStatus.Failed, null);

        public Func<AuthSession> RenewHandler { get; set; } =
            () => throw new TerminalApiException(TerminalApiErrorKind.Transport, "no renew configured");

        public List<(string User, string Pass)> SignInCalls { get; } = [];

        public int RenewCount { get; private set; }

        public Task<SignInOutcome> SignInAsync(string username, string password, CancellationToken cancellationToken)
        {
            lock (_gate)
            {
                SignInCalls.Add((username, password));
            }

            return System.Threading.Tasks.Task.FromResult(SignInHandler(username, password));
        }

        public Task<AuthSession> RenewTokenAsync(CancellationToken cancellationToken)
        {
            lock (_gate)
            {
                RenewCount++;
            }

            return System.Threading.Tasks.Task.FromResult(RenewHandler());
        }
    }

    private sealed class FakeTaskApi : ITaskApiClient
    {
        private readonly object _gate = new();
        private TaskCompletionSource? _saveGate;

        public Func<IReadOnlyList<TerminalTask>> LoadHandler { get; set; } = () => [];

        public Func<IReadOnlyCollection<TerminalTask>, IReadOnlyList<TerminalTask>> SaveHandler { get; set; } = Echo;

        public IReadOnlyList<TerminalTask> LoadResult
        {
            set => LoadHandler = () => value;
        }

        public List<List<TerminalTask>> SavedBatches { get; } = [];

        public int LoadCount { get; private set; }

        public CancellationToken LastSaveToken { get; private set; }

        public void HoldSaves()
        {
            _saveGate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public Task<IReadOnlyList<TerminalTask>> LoadTasksAsync(CancellationToken cancellationToken)
        {
            lock (_gate)
            {
                LoadCount++;
            }

            return System.Threading.Tasks.Task.FromResult(LoadHandler());
        }

        public async Task<IReadOnlyList<TerminalTask>> SaveTasksAsync(
            IReadOnlyCollection<TerminalTask> tasks, CancellationToken cancellationToken)
        {
            TaskCompletionSource? gate;
            lock (_gate)
            {
                SavedBatches.Add([.. tasks]);
                LastSaveToken = cancellationToken;
                gate = _saveGate;
            }

            if (gate is not null)
            {
                await gate.Task.WaitAsync(cancellationToken);
            }

            return SaveHandler(tasks);
        }
    }

    private sealed class FakeHub : ITaskHubClient
    {
        private readonly object _gate = new();

        public Action<TaskHubEvent>? Sink { get; set; }

        public List<TaskHubEvent> Buffered { get; } = [];

        public int StartCount { get; private set; }

        public int StopCount { get; private set; }

        public int DisposeCount { get; private set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            lock (_gate)
            {
                StartCount++;
            }

            return System.Threading.Tasks.Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            lock (_gate)
            {
                StopCount++;
            }

            return System.Threading.Tasks.Task.CompletedTask;
        }

        public Task ReconnectNowAsync(CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public IReadOnlyList<TaskHubEvent> DrainBufferedUpdates()
        {
            lock (_gate)
            {
                TaskHubEvent[] events = [.. Buffered];
                Buffered.Clear();
                return events;
            }
        }

        public ValueTask DisposeAsync()
        {
            lock (_gate)
            {
                DisposeCount++;
            }

            return ValueTask.CompletedTask;
        }

        public void Raise(TaskHubEvent hubEvent)
        {
            Sink?.Invoke(hubEvent);
        }
    }

    private sealed class InMemoryStateStore : ILocalStateStore
    {
        private PersistedTerminalState? _state;

        public PersistedTerminalState? Saved { get; private set; }

        public TerminalStateException? LoadFault { get; set; }

        public void Set(PersistedTerminalState state)
        {
            _state = state;
        }

        public PersistedTerminalState? Load()
        {
            if (LoadFault is not null)
            {
                throw LoadFault;
            }

            return _state;
        }

        public void Save(PersistedTerminalState state)
        {
            _state = state;
            Saved = state;
        }
    }

    private sealed class InMemoryTokenStore : ITokenStore
    {
        private string? _token;

        public string? Load()
        {
            return _token;
        }

        public void Save(string token)
        {
            _token = token;
        }

        public void Clear()
        {
            _token = null;
        }
    }

    private sealed class BlockingKeySource : IKeyInputSource
    {
        public Task<ConsoleKeyInfo?> ReadKeyAsync(CancellationToken cancellationToken)
        {
            var completion = new TaskCompletionSource<ConsoleKeyInfo?>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            cancellationToken.Register(() => completion.TrySetCanceled(cancellationToken));
            return completion.Task;
        }
    }

    private sealed class CaptureRenderer : ITerminalRenderer
    {
        private readonly object _gate = new();
        private readonly List<Frame> _frames = [];

        public int BeginCount { get; private set; }

        public int EndCount { get; private set; }

        public int ThrowAfter { get; set; } = -1;

        public void Begin()
        {
            lock (_gate)
            {
                BeginCount++;
            }
        }

        public void Render(TerminalViewModel model, int width, int height, bool resizeRequired)
        {
            lock (_gate)
            {
                if (ThrowAfter >= 0 && _frames.Count >= ThrowAfter)
                {
                    throw new InvalidOperationException("Simulated render failure.");
                }

                _frames.Add(new Frame(model, resizeRequired));
            }
        }

        public void End()
        {
            lock (_gate)
            {
                EndCount++;
            }
        }

        public Frame? Last()
        {
            lock (_gate)
            {
                return _frames.Count == 0 ? null : _frames[^1];
            }
        }
    }

    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            return now;
        }
    }

    private sealed class FixedDates : ILocalDateProvider, ITaskTextDateProvider
    {
        public DateOnly Today => new(2024, 6, 10);
    }
}
