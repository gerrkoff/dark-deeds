namespace DD.TerminalClient.Details.Ui;

// The rendering-only description of the frame's status area. Normal shows the focused task and key
// hints; Editor and Login show a prompt plus the current line-editor buffer and cursor (Login masks
// the buffer); Confirmation shows the yes/no question in Prompt; Help renders the keymap. Hint carries
// optional secondary text such as live parser feedback. The renderer never mutates this value and never
// runs a Spectre prompt: it only draws the supplied buffer and cursor into the live frame.
public sealed record TerminalStatus
{
    public TerminalStatusKind Kind { get; init; }

    public string Prompt { get; init; } = string.Empty;

    public string Input { get; init; } = string.Empty;

    public int CursorPosition { get; init; }

    public bool MaskInput { get; init; }

    public string? Hint { get; init; }
}
