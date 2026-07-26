using DD.TerminalClient.Domain.Navigation;
using DD.TerminalClient.Domain.Overview;

namespace DD.TerminalClient.Details.Ui;

// The complete immutable input to the terminal frame renderer: the projected Overview, the current
// focus, the local date used to highlight today, the local completed-visibility toggle, the connection
// and conflict indicators, the active profile name and the status area to draw. The event loop maps its
// application state onto this value once per drained batch; the renderer reads it and produces the frame.
public sealed record TerminalViewModel
{
    public required OverviewProjection Overview { get; init; }

    public TaskFocus? Focus { get; init; }

    public required DateOnly Today { get; init; }

    public bool ShowCompleted { get; init; }

    public bool IsOffline { get; init; }

    public string? Notification { get; init; }

    public string ProfileName { get; init; } = string.Empty;

    public TerminalStatus Status { get; init; } = new();
}
