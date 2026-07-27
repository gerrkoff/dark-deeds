namespace DD.TerminalClient.Domain.Input;

// The minimal facts about the focused task the reducer needs to honour the keymap's context rules: is a
// task focused at all, does it carry a date (so a day-move or Routine toggle is meaningful), and the
// text to seed the edit and move editors plus the date an add inherits. The application, which owns the
// projection and focus, fills this in for every key press.
public sealed record TerminalInputContext
{
    public static readonly TerminalInputContext None = new();

    public bool HasFocus { get; init; }

    public bool FocusHasDate { get; init; }

    public DateOnly? FocusDate { get; init; }

    // The focused task's Uid, captured when an edit/move/delete modal opens so its later commit targets the
    // task the user chose rather than whatever the focus has since moved to (a realtime server-wins update
    // can drop the focused task and slide focus onto a neighbour while the modal is open).
    public string? FocusUid { get; init; }

    public string EditText { get; init; } = string.Empty;

    public string MoveText { get; init; } = string.Empty;
}
