using DD.TerminalClient.Domain.Input;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Navigation;
using DD.TerminalClient.Domain.Overview;

namespace DD.TerminalClient.Domain.Application;

// The lifecycle phase the application is in. Login collects credentials; ConfirmDataReset asks before
// discarding a previous user's cached data on this profile; Ready is normal interactive operation.
public enum ApplicationPhase
{
    Login,
    ConfirmDataReset,
    Ready,
}

// Which field the two-step masked login is collecting. The username step is rendered unmasked, the
// password step masked; the reducer advances Username to Password on the first Enter.
public enum LoginStep
{
    Username,
    Password,
}

// The complete in-memory application and UI state, owned and mutated only by the single-reader event
// loop. It is an immutable value: every transition returns a copy. It carries the durable task cache
// (the whole projection input, deleted tasks included), the local view toggles, the recomputed Overview
// projection and keyboard focus, the connectivity/sync indicators, the input-layer state, the terminal
// dimensions and the login sub-state. The save queues live in TaskSyncCoordinator, not here.
public sealed record ApplicationState
{
    private static readonly OverviewProjection EmptyProjection = new()
    {
        NoDate = new OverviewDay { Section = OverviewSection.NoDate, Row = 0, Column = 0 },
    };

    public ApplicationPhase Phase { get; init; } = ApplicationPhase.Login;

    // Set by the Quit command/effect; the loop finishes the current batch, then exits and runs cleanup.
    public bool Quit { get; init; }

    // The whole task cache, including soft-deleted records, exactly as the projection consumes it.
    public IReadOnlyList<TerminalTask> Cache { get; init; } = [];

    public bool ShowCompleted { get; init; }

    // When true, Routine tasks are expanded on every dated day; otherwise every dated day collapses them.
    public bool ShowRoutineTasks { get; init; }

    // Derived from Cache + toggles by the reducer's Recompute; the renderer reads it directly.
    public OverviewProjection Projection { get; init; } = EmptyProjection;

    public TaskFocus? Focus { get; init; }

    public bool IsOffline { get; init; }

    // True while a snapshot load is in progress and the hub is buffering incoming pushes for replay.
    public bool IsBuffering { get; init; }

    public bool IsSaving { get; init; }

    public TerminalInputState Input { get; init; } = TerminalInputState.Normal;

    // The interaction (an open editor or delete confirmation and its draft) suspended when the terminal
    // dropped below the supported minimum, kept so growing back restores it instead of discarding it into
    // resize-required. Null whenever nothing is suspended (a usable size, or an already-resumed session).
    public TerminalInputState? SuspendedInput { get; init; }

    // A conflict/server-update notice that persists until replaced by a newer server event.
    public string? Notification { get; init; }

    // A transient status line for the most recent action (declined command, "Signing in...", etc.).
    public string? StatusMessage { get; init; }

    public int Width { get; init; }

    public int Height { get; init; }

    public LoginStep LoginStep { get; init; } = LoginStep.Username;

    // True from the moment a password is submitted until sign-in resolves. While set, the login reducer
    // ignores keys so a second submission cannot start a concurrent sign-in whose out-of-order completion
    // could bounce an already-authenticated session back to the login screen.
    public bool SigningIn { get; init; }

    public string? PendingUsername { get; init; }

    // The user signing in and the previous data owner, shown by the different-user confirmation prompt.
    public string? ConfirmUser { get; init; }

    public string? ConfirmOwner { get; init; }
}
