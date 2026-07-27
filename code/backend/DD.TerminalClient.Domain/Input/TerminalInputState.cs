using DD.TerminalClient.Domain.Editing;

namespace DD.TerminalClient.Domain.Input;

// Why an editor is open, so a commit knows which application command to raise and whether to mask and
// show live parser feedback. AddWithFocusDate seeds the new task from the focused task's date; AddNoDate
// creates a No Date task; Edit and Move act on the focused task; Login collects a masked credential.
public enum EditorPurpose
{
    None,
    AddWithFocusDate,
    AddNoDate,
    Edit,
    Move,
    Login,
}

// The complete input-layer state the application carries between key presses: the active mode, the open
// editor's purpose and buffer, the live parser feedback (null outside the add/edit editors) and the
// fallback date an AddWithFocusDate preview applies. It owns no application data - focus, tasks and the
// terminal size live in the application state - so it stays a small, deterministic value.
public sealed record TerminalInputState
{
    public static readonly TerminalInputState Normal = new();

    public TerminalUiMode Mode { get; init; } = TerminalUiMode.Normal;

    public EditorPurpose Purpose { get; init; } = EditorPurpose.None;

    public LineEditorState Editor { get; init; } = LineEditorState.Empty;

    // The Uid of the task an edit/move/delete modal acts on, captured when the modal opens so the commit
    // resolves that exact task instead of the current focus (which a realtime update can move mid-modal).
    // Null outside those modals.
    public string? TargetUid { get; init; }

    public string? Feedback { get; init; }

    public DateOnly? FallbackDate { get; init; }

    // Enters masked login mode with an empty buffer. Login is application-driven (startup or a 401), not
    // reachable from a normal-mode key, so the application and its tests build it through this factory.
    public static TerminalInputState BeginLogin()
    {
        return new TerminalInputState { Mode = TerminalUiMode.MaskedLogin, Purpose = EditorPurpose.Login };
    }
}
