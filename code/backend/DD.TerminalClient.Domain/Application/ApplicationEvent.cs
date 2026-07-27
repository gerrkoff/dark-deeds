using DD.TerminalClient.Domain.Authentication;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Realtime;

namespace DD.TerminalClient.Domain.Application;

// The kinds of message the application's single event channel carries. Every producer - the keyboard
// reader, the SignalR hub sink, the REST/renew/save completions, the retry and renewal timers, and the
// resize watcher - only enqueues one of these; the loop is the sole reader and the sole writer of state.
public enum ApplicationEventKind
{
    Key,
    Resize,
    Hub,
    SnapshotLoaded,
    SnapshotFailed,
    SignInCompleted,
    SignInFaulted,
    SaveCompleted,
    SaveFaulted,
    RetryTick,
    ReloadSnapshotTick,
    RenewTick,
    RenewCompleted,
    RenewFaulted,
}

// One message on the application event channel, carrying only the payload its Kind needs. Built through
// the static factories so a producer sets exactly the fields meaningful for the kind, and the loop
// switches on Kind and reads the matching payload. Immutable and free of any I/O type.
public sealed record ApplicationEvent
{
    private static readonly IReadOnlyList<TerminalTask> NoTasks = [];

    public ApplicationEventKind Kind { get; private init; }

    // For Key, the pressed key. Default for every other kind.
    public ConsoleKeyInfo Key { get; private init; }

    // For Resize, the new terminal dimensions.
    public int Width { get; private init; }

    public int Height { get; private init; }

    // For Hub, the translated real-time event.
    public TaskHubEvent? Hub { get; private init; }

    // For SnapshotLoaded and SaveCompleted, the tasks the server returned.
    public IReadOnlyList<TerminalTask> Tasks { get; private init; } = NoTasks;

    // For SnapshotFailed, SaveFaulted and RenewFaulted, whether the failure was an authentication (401).
    public bool Unauthorized { get; private init; }

    // For SignInCompleted, the sign-in outcome.
    public SignInOutcome? SignIn { get; private init; }

    // For SnapshotLoaded and SnapshotFailed, the monotonically increasing load generation the completion
    // belongs to, so the loop can discard a stale result from a superseded (overlapping) snapshot load.
    public int Generation { get; private init; }

    // For SignInFaulted, a secret-safe reason to show the user.
    public string Message { get; private init; } = string.Empty;

    // For RenewCompleted, the refreshed session.
    public AuthSession? Session { get; private init; }

    public static ApplicationEvent RetryTick { get; } = new() { Kind = ApplicationEventKind.RetryTick };

    public static ApplicationEvent ReloadSnapshotTick { get; } =
        new() { Kind = ApplicationEventKind.ReloadSnapshotTick };

    public static ApplicationEvent RenewTick { get; } = new() { Kind = ApplicationEventKind.RenewTick };

    public static ApplicationEvent KeyPressed(ConsoleKeyInfo key)
    {
        return new ApplicationEvent { Kind = ApplicationEventKind.Key, Key = key };
    }

    public static ApplicationEvent Resized(int width, int height)
    {
        return new ApplicationEvent { Kind = ApplicationEventKind.Resize, Width = width, Height = height };
    }

    public static ApplicationEvent HubEvent(TaskHubEvent hubEvent)
    {
        ArgumentNullException.ThrowIfNull(hubEvent);
        return new ApplicationEvent { Kind = ApplicationEventKind.Hub, Hub = hubEvent };
    }

    public static ApplicationEvent SnapshotLoaded(IReadOnlyList<TerminalTask> tasks, int generation)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        return new ApplicationEvent
        {
            Kind = ApplicationEventKind.SnapshotLoaded,
            Tasks = tasks,
            Generation = generation,
        };
    }

    public static ApplicationEvent SnapshotFailed(bool unauthorized, int generation)
    {
        return new ApplicationEvent
        {
            Kind = ApplicationEventKind.SnapshotFailed,
            Unauthorized = unauthorized,
            Generation = generation,
        };
    }

    public static ApplicationEvent SignInCompleted(SignInOutcome outcome)
    {
        ArgumentNullException.ThrowIfNull(outcome);
        return new ApplicationEvent { Kind = ApplicationEventKind.SignInCompleted, SignIn = outcome };
    }

    public static ApplicationEvent SignInFaulted(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        return new ApplicationEvent { Kind = ApplicationEventKind.SignInFaulted, Message = message };
    }

    public static ApplicationEvent SaveCompleted(IReadOnlyList<TerminalTask> saved)
    {
        ArgumentNullException.ThrowIfNull(saved);
        return new ApplicationEvent { Kind = ApplicationEventKind.SaveCompleted, Tasks = saved };
    }

    public static ApplicationEvent SaveFaulted(bool unauthorized)
    {
        return new ApplicationEvent { Kind = ApplicationEventKind.SaveFaulted, Unauthorized = unauthorized };
    }

    public static ApplicationEvent RenewCompleted(AuthSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        return new ApplicationEvent { Kind = ApplicationEventKind.RenewCompleted, Session = session };
    }

    public static ApplicationEvent RenewFaulted(bool unauthorized)
    {
        return new ApplicationEvent { Kind = ApplicationEventKind.RenewFaulted, Unauthorized = unauthorized };
    }
}
