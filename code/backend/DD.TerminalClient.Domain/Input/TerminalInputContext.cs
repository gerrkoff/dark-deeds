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

    public string EditText { get; init; } = string.Empty;

    public string MoveText { get; init; } = string.Empty;
}
