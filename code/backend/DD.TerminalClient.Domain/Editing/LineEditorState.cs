namespace DD.TerminalClient.Domain.Editing;

// A pure single-line text buffer with a cursor, used by every text mode (add, edit, move and masked
// login). Every operation returns a new value and never mutates in place; the cursor is always clamped
// into [0, Text.Length] so an out-of-range cursor supplied from persisted state can never index outside
// the string. Insert accepts a whole chunk so a single typed character and a multi-character paste share
// one code path. It knows nothing about parsing, masking or rendering: the reducer drives it and the
// renderer reads Text and Cursor.
public sealed record LineEditorState
{
    public static readonly LineEditorState Empty = new();

    public string Text { get; init; } = string.Empty;

    public int Cursor { get; init; }

    // Starts editing an existing value with the cursor parked at the end, matching how the web editor
    // opens a task for editing.
    public static LineEditorState For(string text)
    {
        var value = text ?? string.Empty;
        return new LineEditorState { Text = value, Cursor = value.Length };
    }

    public LineEditorState Insert(string chunk)
    {
        if (string.IsNullOrEmpty(chunk))
            return this;

        var cursor = Clamp(Cursor);
        return new LineEditorState
        {
            Text = string.Concat(Text[..cursor], chunk, Text[cursor..]),
            Cursor = cursor + chunk.Length,
        };
    }

    public LineEditorState Backspace()
    {
        var cursor = Clamp(Cursor);
        if (cursor == 0)
            return this;

        return new LineEditorState
        {
            Text = string.Concat(Text[..(cursor - 1)], Text[cursor..]),
            Cursor = cursor - 1,
        };
    }

    public LineEditorState Delete()
    {
        var cursor = Clamp(Cursor);
        if (cursor >= Text.Length)
            return this;

        return new LineEditorState
        {
            Text = string.Concat(Text[..cursor], Text[(cursor + 1)..]),
            Cursor = cursor,
        };
    }

    public LineEditorState MoveLeft()
    {
        return this with { Cursor = Math.Max(0, Clamp(Cursor) - 1) };
    }

    public LineEditorState MoveRight()
    {
        return this with { Cursor = Math.Min(Text.Length, Clamp(Cursor) + 1) };
    }

    public LineEditorState Home()
    {
        return this with { Cursor = 0 };
    }

    public LineEditorState End()
    {
        return this with { Cursor = Text.Length };
    }

    private int Clamp(int cursor)
    {
        return Math.Clamp(cursor, 0, Text.Length);
    }
}
