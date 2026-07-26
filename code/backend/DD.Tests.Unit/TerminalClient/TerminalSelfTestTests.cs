using System.Text;
using DD.TerminalClient.Domain.Abstractions;
using DD.TerminalClient.Domain.Authentication;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Realtime;
using DD.TerminalClient.SelfTest;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Drives the unattended self-test against fake auth/task/hub adapters. The fake backend applies the real
// server's version and delete semantics and, when broadcasting is enabled, pushes the resulting update to
// the observer hub so the contract assertions have something to observe. Every run carries an explicit
// timeout so a broken assertion can never hang the suite.
public sealed class TerminalSelfTestTests
{
    private static readonly TimeSpan RunTimeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task RunAsync_HappyPath_CompletesTheContractAndReturnsSuccess()
    {
        var harness = new Harness();

        var exitCode = await new TerminalSelfTest(harness.BuildContext()).RunAsync(CancellationToken.None)
            .WaitAsync(RunTimeout);

        Assert.Equal(TerminalSelfTest.SuccessExitCode, exitCode);
        Assert.Contains("[self-test] PASSED", harness.Reports);
        Assert.True(harness.Backend.IsDeleted("probe-uid"), "the probe task should be deleted at the end");
    }

    [Fact]
    public async Task RunAsync_HappyPath_ObservesEveryLifeCycleStage()
    {
        var harness = new Harness();

        await new TerminalSelfTest(harness.BuildContext()).RunAsync(CancellationToken.None).WaitAsync(RunTimeout);

        Assert.Contains(harness.Reports, line => line.Contains("create: observer received", StringComparison.Ordinal));
        Assert.Contains(harness.Reports, line => line.Contains("complete: observer received", StringComparison.Ordinal));
        Assert.Contains(harness.Reports, line => line.Contains("observer received the deletion", StringComparison.Ordinal));
    }

    [Fact]
    public async Task RunAsync_ObserverUpdateNeverArrives_TimesOutWithStageDiagnostics()
    {
        var harness = new Harness { BroadcastEnabled = false };

        var exitCode = await new TerminalSelfTest(harness.BuildContext(TimeSpan.FromMilliseconds(150)))
            .RunAsync(CancellationToken.None)
            .WaitAsync(RunTimeout);

        Assert.Equal(TerminalSelfTest.FailureExitCode, exitCode);
        Assert.Contains(
            harness.Reports,
            line => line.Contains("create task (observer update)", StringComparison.Ordinal)
                && line.Contains("timed out", StringComparison.Ordinal));
    }

    [Fact]
    public async Task RunAsync_FailsAfterCreate_SoftDeletesTheProbeInFinally()
    {
        var harness = new Harness();
        harness.Backend.FailWhen = tasks => tasks.Any(task => task is { Completed: true, Deleted: false });

        var exitCode = await new TerminalSelfTest(harness.BuildContext()).RunAsync(CancellationToken.None)
            .WaitAsync(RunTimeout);

        Assert.Equal(TerminalSelfTest.FailureExitCode, exitCode);
        Assert.Contains(harness.Reports, line => line.Contains("complete task (REST)", StringComparison.Ordinal));
        Assert.True(harness.Backend.IsDeleted("probe-uid"), "cleanup should remove the probe after the failure");
        Assert.Contains(harness.Reports, line => line.Contains("cleanup: removed", StringComparison.Ordinal));
    }

    [Fact]
    public async Task RunAsync_SignInRejected_ReturnsFailureWithoutTouchingHubs()
    {
        var harness = new Harness();
        harness.Auth.SignInHandler = (_, _) => new SignInOutcome(TerminalSignInStatus.InvalidCredentials, null);

        var exitCode = await new TerminalSelfTest(harness.BuildContext()).RunAsync(CancellationToken.None)
            .WaitAsync(RunTimeout);

        Assert.Equal(TerminalSelfTest.FailureExitCode, exitCode);
        Assert.Contains(harness.Reports, line => line.Contains("FAILED at sign-in", StringComparison.Ordinal));
        Assert.Equal(0, harness.Observer.StartCount);
    }

    [Fact]
    public async Task RunAsync_HappyPath_NeverEmitsCredentialsOrToken()
    {
        var harness = new Harness();

        await new TerminalSelfTest(harness.BuildContext()).RunAsync(CancellationToken.None).WaitAsync(RunTimeout);

        Assert.NotEmpty(harness.Reports);
        Assert.DoesNotContain(harness.Reports, line => line.Contains(Harness.Password, StringComparison.Ordinal));
        Assert.DoesNotContain(harness.Reports, line => line.Contains(harness.Token!, StringComparison.Ordinal));
    }

    [Fact]
    public void Redact_MasksEveryKnownSecret()
    {
        var line = "signed in as tester with token abc.def.ghi and password s3cr3t";

        var redacted = TerminalSelfTest.Redact(line, ["abc.def.ghi", "s3cr3t"]);

        Assert.DoesNotContain("abc.def.ghi", redacted, StringComparison.Ordinal);
        Assert.DoesNotContain("s3cr3t", redacted, StringComparison.Ordinal);
        Assert.Contains("***", redacted, StringComparison.Ordinal);
    }

    [Fact]
    public void TryReadCredentials_MissingVariables_ReturnsFalseWithSafeError()
    {
        var environment = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["DD_TERMINAL_USERNAME"] = "someone",
        };

        var read = TerminalSelfTest.TryReadCredentials(
            key => environment.GetValueOrDefault(key), out var credentials, out var error);

        Assert.False(read);
        Assert.Null(credentials);
        Assert.Contains("DD_TERMINAL_PASSWORD", error, StringComparison.Ordinal);
        Assert.DoesNotContain("someone", error, StringComparison.Ordinal);
    }

    [Fact]
    public void TryReadCredentials_AllPresent_ReturnsCredentials()
    {
        var environment = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["DD_TERMINAL_USERNAME"] = "someone",
            ["DD_TERMINAL_PASSWORD"] = "secret",
        };

        var read = TerminalSelfTest.TryReadCredentials(
            key => environment.GetValueOrDefault(key), out var credentials, out var error);

        Assert.True(read);
        Assert.Null(error);
        Assert.Equal("someone", credentials!.Username);
        Assert.Equal("secret", credentials.Password);
    }

    private static string Jwt(string user)
    {
        static string Segment(string json)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json))
                .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        var header = Segment("{\"alg\":\"none\"}");
        var payload = Segment($"{{\"name\":\"{user}\",\"exp\":4102444800}}");
        return $"{header}.{payload}.signature";
    }

    private sealed class Harness
    {
        public const string Password = "s3cr3t-pass";

        public Harness()
        {
            Token = Jwt("tester");
            Auth.SignInHandler = (_, _) =>
                new SignInOutcome(TerminalSignInStatus.Success, AuthSession.FromToken(Token));
            Backend.OnSaved = saved =>
            {
                if (BroadcastEnabled)
                {
                    Observer.Raise(TaskHubEvent.Update(saved));
                }
            };
        }

        public FakeAuthApi Auth { get; } = new();

        public FakeBackend Backend { get; } = new();

        public FakeHub Observer { get; } = new();

        public FakeHub Writer { get; } = new();

        public List<string> Reports { get; } = [];

        public string? Token { get; }

        public bool BroadcastEnabled { get; init; } = true;

        public SelfTestContext BuildContext(TimeSpan? eventTimeout = null)
        {
            return new SelfTestContext
            {
                Auth = Auth,
                Writer = Backend,
                ObserverHubFactory = sink =>
                {
                    Observer.Sink = sink;
                    return Observer;
                },
                WriterHubFactory = sink =>
                {
                    Writer.Sink = sink;
                    return Writer;
                },
                SetToken = _ => { },
                Report = line =>
                {
                    lock (Reports)
                    {
                        Reports.Add(line);
                    }
                },
                Credentials = new SelfTestCredentials("tester", Password),
                NewUid = () => "probe-uid",
                NewTitle = () => "probe-title",
                EventTimeout = eventTimeout ?? TimeSpan.FromSeconds(5),
            };
        }
    }

    private sealed class FakeAuthApi : IAuthApiClient
    {
        public Func<string, string, SignInOutcome> SignInHandler { get; set; } =
            (_, _) => new SignInOutcome(TerminalSignInStatus.Failed, null);

        public Task<SignInOutcome> SignInAsync(string username, string password, CancellationToken cancellationToken)
        {
            return Task.FromResult(SignInHandler(username, password));
        }

        public Task<AuthSession> RenewTokenAsync(CancellationToken cancellationToken)
        {
            throw new TerminalApiException(TerminalApiErrorKind.Transport, "renew is not used by the self-test.");
        }
    }

    // Applies the real backend's version and delete semantics so the contract assertions are meaningful:
    // a new task is stored at version 1, an update increments the version, and a delete marks the stored
    // task deleted with an incremented version. Each successful save is broadcast through OnSaved.
    private sealed class FakeBackend : ITaskApiClient
    {
        private readonly Dictionary<string, TerminalTask> _store = new(StringComparer.Ordinal);

        public Action<IReadOnlyList<TerminalTask>>? OnSaved { get; set; }

        public Func<IReadOnlyCollection<TerminalTask>, bool>? FailWhen { get; set; }

        public Task<IReadOnlyList<TerminalTask>> LoadTasksAsync(CancellationToken cancellationToken)
        {
            IReadOnlyList<TerminalTask> snapshot = _store.Values.Where(task => !task.Deleted).ToList();
            return Task.FromResult(snapshot);
        }

        public Task<IReadOnlyList<TerminalTask>> SaveTasksAsync(
            IReadOnlyCollection<TerminalTask> tasks, CancellationToken cancellationToken)
        {
            if (FailWhen?.Invoke(tasks) == true)
            {
                throw new TerminalApiException(TerminalApiErrorKind.Transport, "the save failed.");
            }

            IReadOnlyList<TerminalTask> saved = tasks.Select(ApplyServerLogic).ToList();
            OnSaved?.Invoke(saved);
            return Task.FromResult(saved);
        }

        public bool IsDeleted(string uid)
        {
            return _store.TryGetValue(uid, out var task) && task.Deleted;
        }

        private TerminalTask ApplyServerLogic(TerminalTask task)
        {
            if (task.Deleted)
            {
                if (_store.TryGetValue(task.Uid, out var existing))
                {
                    var removed = existing with { Deleted = true, Version = existing.Version + 1 };
                    _store[task.Uid] = removed;
                    return removed;
                }

                return task with { Deleted = true, Version = task.Version + 1 };
            }

            if (!_store.TryGetValue(task.Uid, out var current))
            {
                var created = task with { Version = 1 };
                _store[task.Uid] = created;
                return created;
            }

            var updated = task with { Version = current.Version + 1 };
            _store[task.Uid] = updated;
            return updated;
        }
    }

    private sealed class FakeHub : ITaskHubClient
    {
        private readonly object _gate = new();

        public Action<TaskHubEvent>? Sink { get; set; }

        public int StartCount { get; private set; }

        public int StopCount { get; private set; }

        public int DisposeCount { get; private set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            lock (_gate)
            {
                StartCount++;
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            lock (_gate)
            {
                StopCount++;
            }

            return Task.CompletedTask;
        }

        public Task ReconnectNowAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public IReadOnlyList<TaskHubEvent> DrainBufferedUpdates()
        {
            return [];
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
}
