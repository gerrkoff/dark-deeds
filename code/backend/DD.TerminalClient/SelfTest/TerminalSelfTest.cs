using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using DD.TerminalClient.Domain.Abstractions;
using DD.TerminalClient.Domain.Authentication;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Realtime;

namespace DD.TerminalClient.SelfTest;

// The unattended contract check. It never enters the alternate screen: it drives the real backend
// through the same adapters the interactive app uses, asserting the full task life cycle against both
// the REST response and a second (observer) hub connection, and reports only secret-safe, stage-labelled
// diagnostics. Every wait carries a finite timeout so a broken backend fails fast instead of hanging,
// and the created probe task is always removed and both hubs stopped in the finally block. The adapters
// and options arrive through SelfTestContext so the orchestration is exercised end to end with fakes.
internal sealed class TerminalSelfTest
{
    public const int SuccessExitCode = 0;
    public const int FailureExitCode = 1;
    public const int UsageExitCode = 2;

    private const string UsernameVariable = "DD_TERMINAL_USERNAME";
    private const string PasswordVariable = "DD_TERMINAL_PASSWORD";

    private readonly SelfTestContext _context;
    private readonly List<string> _secrets = [];

    private readonly Channel<TaskHubEvent> _observerEvents =
        Channel.CreateUnbounded<TaskHubEvent>(new UnboundedChannelOptions { SingleReader = true });

    private ITaskHubClient? _observerHub;

    public TerminalSelfTest(SelfTestContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    // Reads and validates the required credentials from an environment accessor. Kept static and
    // accessor-driven so the missing-variable path is unit-testable without touching the real process
    // environment; the error names only the missing variables, never any value.
    public static bool TryReadCredentials(
        Func<string, string?> getEnvironmentVariable,
        [NotNullWhen(true)] out SelfTestCredentials? credentials,
        [NotNullWhen(false)] out string? error)
    {
        ArgumentNullException.ThrowIfNull(getEnvironmentVariable);

        var username = getEnvironmentVariable(UsernameVariable);
        var password = getEnvironmentVariable(PasswordVariable);

        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(username))
        {
            missing.Add(UsernameVariable);
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            missing.Add(PasswordVariable);
        }

        if (missing.Count > 0)
        {
            credentials = null;
            error = $"--self-test requires {string.Join(" and ", missing)} to be set.";
            return false;
        }

        credentials = new SelfTestCredentials(username!, password!);
        error = null;
        return true;
    }

    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        AddSecret(_context.Credentials.Password);

        ITaskHubClient? observerHub = null;
        ITaskHubClient? writerHub = null;
        string? probeUid = null;
        string? probeTitle = null;
        var removed = false;

        try
        {
            await SignInAsync(cancellationToken);

            observerHub = _context.ObserverHubFactory(hubEvent => _observerEvents.Writer.TryWrite(hubEvent));
            writerHub = _context.WriterHubFactory(_ => { });
            _observerHub = observerHub;
            await StartHubAsync(observerHub, "observer hub", cancellationToken);
            await StartHubAsync(writerHub, "writer hub", cancellationToken);
            Report("hubs: writer and observer started with distinct client ids");

            var snapshot = await LoadSnapshotAsync(cancellationToken);
            Report($"snapshot: loaded {snapshot.Count} task(s)");

            probeUid = _context.NewUid();
            probeTitle = _context.NewTitle();

            var created = await CreateProbeAsync(probeUid, probeTitle, cancellationToken);
            var completed = await CompleteProbeAsync(created, cancellationToken);
            await DeleteProbeAsync(completed, cancellationToken);
            removed = true;

            Report("PASSED");
            return SuccessExitCode;
        }
        catch (SelfTestException failure)
        {
            Report($"FAILED at {failure.Stage}: {failure.Message}");
            return FailureExitCode;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Report("CANCELLED before completion");
            return FailureExitCode;
        }
        finally
        {
            await CleanupAsync(probeUid, probeTitle, removed, observerHub, writerHub);
        }
    }

    // Removes any known secret (the password and, once obtained, the JWT) from a diagnostic line. The
    // username is never emitted by construction, so it is not scrubbed here to avoid corrupting ordinary
    // words in the diagnostics; only the high-entropy secrets are masked as defense in depth.
    internal static string Redact(string line, IReadOnlyCollection<string> secrets)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentNullException.ThrowIfNull(secrets);

        var result = line;
        foreach (var secret in secrets)
        {
            if (!string.IsNullOrEmpty(secret))
            {
                result = result.Replace(secret, "***", StringComparison.Ordinal);
            }
        }

        return result;
    }

    private static async Task StartHubAsync(ITaskHubClient hub, string stage, CancellationToken cancellationToken)
    {
        try
        {
            await hub.StartAsync(cancellationToken);
        }
        catch (TerminalApiException exception)
        {
            throw new SelfTestException(stage, DescribeApiError(exception), exception);
        }

        // Leave buffering mode so live server pushes reach the sink; any pre-write buffered updates are
        // discarded because the probe does not exist yet.
        hub.DrainBufferedUpdates();
    }

    private static async Task StopHubQuietlyAsync(ITaskHubClient? hub, CancellationToken cancellationToken)
    {
        if (hub is null)
        {
            return;
        }

        try
        {
            await hub.StopAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Ignored during teardown.
        }
        catch (ObjectDisposedException)
        {
            // Ignored during teardown.
        }
        finally
        {
            await hub.DisposeAsync();
        }
    }

    // Only the coarse failure class is reported; the exception message never carries a body or token, but
    // the class alone is enough to identify the failed contract stage without leaking anything.
    private static string DescribeApiError(TerminalApiException exception)
    {
        return $"the backend call failed ({exception.Kind}).";
    }

    private async Task SignInAsync(CancellationToken cancellationToken)
    {
        SignInOutcome outcome;
        try
        {
            outcome = await _context.Auth.SignInAsync(
                _context.Credentials.Username, _context.Credentials.Password, cancellationToken);
        }
        catch (TerminalApiException exception)
        {
            throw new SelfTestException("sign-in", DescribeApiError(exception), exception);
        }

        if (outcome.Status != TerminalSignInStatus.Success || outcome.Session is null)
        {
            throw new SelfTestException("sign-in", $"the server rejected the sign-in ({outcome.Status}).", null);
        }

        AddSecret(outcome.Session.Token);
        _context.SetToken(outcome.Session.Token);
        Report("sign-in: authenticated");
    }

    private async Task<IReadOnlyList<TerminalTask>> LoadSnapshotAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Writer.LoadTasksAsync(cancellationToken);
        }
        catch (TerminalApiException exception)
        {
            throw new SelfTestException("load snapshot", DescribeApiError(exception), exception);
        }
    }

    private async Task<TerminalTask> CreateProbeAsync(string uid, string title, CancellationToken cancellationToken)
    {
        var probe = new TerminalTask { Uid = uid, Title = title, Type = TerminalTaskType.Simple };
        var created = await SaveAsync(probe, "create task (REST)", cancellationToken);

        if (created.Version != 1)
        {
            throw new SelfTestException(
                "create task (REST)", $"the server assigned version {created.Version}, expected 1.", null);
        }

        Report($"create: REST assigned version {created.Version}");
        await WaitForObserverAsync(
            uid, task => !task.Deleted, "create task (observer update)", cancellationToken);
        Report("create: observer received the update");
        return created;
    }

    private async Task<TerminalTask> CompleteProbeAsync(TerminalTask created, CancellationToken cancellationToken)
    {
        var completed = await SaveAsync(created with { Completed = true }, "complete task (REST)", cancellationToken);

        if (completed.Version <= created.Version)
        {
            throw new SelfTestException("complete task (REST)", "the version did not increment on update.", null);
        }

        if (!completed.Completed)
        {
            throw new SelfTestException("complete task (REST)", "the completed flag was not persisted.", null);
        }

        Report($"complete: REST incremented version to {completed.Version}");
        await WaitForObserverAsync(
            created.Uid,
            task => task.Completed && task.Version >= completed.Version,
            "complete task (observer update)",
            cancellationToken);
        Report("complete: observer received the update");
        return completed;
    }

    private async Task DeleteProbeAsync(TerminalTask completed, CancellationToken cancellationToken)
    {
        var deleted = await SaveAsync(completed with { Deleted = true }, "delete task (REST)", cancellationToken);

        if (!deleted.Deleted)
        {
            throw new SelfTestException("delete task (REST)", "the task was not marked deleted.", null);
        }

        if (deleted.Version <= completed.Version)
        {
            throw new SelfTestException("delete task (REST)", "the version did not increment on delete.", null);
        }

        Report($"delete: REST marked the task deleted at version {deleted.Version}");
        await WaitForObserverAsync(
            completed.Uid, task => task.Deleted, "delete task (observer update)", cancellationToken);
        Report("delete: observer received the deletion update");
    }

    private async Task<TerminalTask> SaveAsync(TerminalTask task, string stage, CancellationToken cancellationToken)
    {
        IReadOnlyList<TerminalTask> response;
        try
        {
            response = await _context.Writer.SaveTasksAsync([task], cancellationToken);
        }
        catch (TerminalApiException exception)
        {
            throw new SelfTestException(stage, DescribeApiError(exception), exception);
        }

        var saved = response.FirstOrDefault(
            candidate => string.Equals(candidate.Uid, task.Uid, StringComparison.Ordinal));
        return saved
            ?? throw new SelfTestException(stage, "the save response did not include the task.", null);
    }

    private async Task WaitForObserverAsync(
        string uid, Func<TerminalTask, bool> predicate, string stage, CancellationToken cancellationToken)
    {
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(_context.EventTimeout);

        try
        {
            while (true)
            {
                var hubEvent = await _observerEvents.Reader.ReadAsync(timeoutCts.Token);

                if (hubEvent.Kind is TaskHubEventKind.Unauthorized or TaskHubEventKind.Closed)
                {
                    throw new SelfTestException(stage, "the observer hub connection was lost.", null);
                }

                if (hubEvent.Kind == TaskHubEventKind.Reconnected)
                {
                    // A reconnect re-enters buffering on the hub, so live pushes stop reaching the sink until
                    // it is drained again (the same reason StartHubAsync drains after the initial connect).
                    // Drain now so subsequent pushes flow, and check anything captured during the reconnect
                    // window against the predicate rather than losing it in the buffer.
                    foreach (var buffered in _observerHub!.DrainBufferedUpdates())
                    {
                        if (MatchesObserverUpdate(buffered, uid, predicate))
                        {
                            return;
                        }
                    }

                    continue;
                }

                if (MatchesObserverUpdate(hubEvent, uid, predicate))
                {
                    return;
                }
            }
        }
        catch (OperationCanceledException)
            when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new SelfTestException(stage, "timed out waiting for the observer update.", null);
        }
    }

    private static bool MatchesObserverUpdate(
        TaskHubEvent hubEvent, string uid, Func<TerminalTask, bool> predicate)
    {
        if (hubEvent.Kind != TaskHubEventKind.Update)
        {
            return false;
        }

        var match = hubEvent.Tasks.FirstOrDefault(
            task => string.Equals(task.Uid, uid, StringComparison.Ordinal));
        return match is not null && predicate(match);
    }

    private async Task CleanupAsync(
        string? probeUid, string? probeTitle, bool removed, ITaskHubClient? observerHub, ITaskHubClient? writerHub)
    {
        // Bound every teardown step so an unresponsive backend or wedged transport fails fast instead of
        // hanging the process (the self-test is an unattended gate), matching the finite-timeout guarantee
        // the rest of this class already keeps.
        using var cleanupCts = new CancellationTokenSource(_context.EventTimeout);

        if (probeUid is not null && !removed)
        {
            var orphan = new TerminalTask { Uid = probeUid, Title = probeTitle ?? probeUid, Deleted = true };
            try
            {
                await _context.Writer.SaveTasksAsync([orphan], cleanupCts.Token);
                Report("cleanup: removed the probe task");
            }
            catch (TerminalApiException)
            {
                Report("cleanup: could not remove the probe task");
            }
            catch (OperationCanceledException)
            {
                Report("cleanup: timed out removing the probe task");
            }
        }

        // Stop both hubs independently so a hang stopping one cannot prevent the other from being torn down.
        await Task.WhenAll(
            StopHubQuietlyAsync(observerHub, cleanupCts.Token),
            StopHubQuietlyAsync(writerHub, cleanupCts.Token));
    }

    private void AddSecret(string? secret)
    {
        if (!string.IsNullOrEmpty(secret))
        {
            _secrets.Add(secret);
        }
    }

    private void Report(string line)
    {
        _context.Report(Redact($"[self-test] {line}", _secrets));
    }
}
