using DD.Shared.TaskText;
using DD.TerminalClient.Domain.Editing;

namespace DD.TerminalClient.Domain.Input;

// The pure input state machine: it maps (current mode + key + focus context) to the next mode, editor
// buffer and application command, with no console, HTTP, clock or file-system dependency. The only
// collaborators are the shared parser and the editor formatter, used solely to render live feedback for
// the add/edit editors. Because it is pure and deterministic, every key, alias, mode transition, invalid
// context, editor operation, commit and cancel is unit-tested by feeding it constructed ConsoleKeyInfo
// values, and the real keyboard is a thin IKeyInputSource adapter around it.
public sealed class TerminalInputReducer(ITaskTextParser parser, TaskTextFormatter formatter)
{
    public TerminalInputResult Reduce(TerminalInputState state, ConsoleKeyInfo key, TerminalInputContext context)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(context);

        return state.Mode switch
        {
            TerminalUiMode.Normal => ReduceNormal(state, key, context),
            TerminalUiMode.Editor => ReduceEditing(state, key),
            TerminalUiMode.MaskedLogin => ReduceEditing(state, key),
            TerminalUiMode.DeleteConfirmation => ReduceConfirmation(state, key),
            TerminalUiMode.Help => ReduceHelp(state, key),
            TerminalUiMode.ResizeRequired => ReduceResizeRequired(state, key),
            _ => TerminalInputResult.Unchanged(state),
        };
    }

    private TerminalInputResult ReduceNormal(TerminalInputState state, ConsoleKeyInfo key, TerminalInputContext context)
    {
        var shift = (key.Modifiers & ConsoleModifiers.Shift) != 0;

        // Arrow keys carry no KeyChar, so they are matched by their ConsoleKey; Shift turns a move into a
        // reorder (vertical) or a one-day move (horizontal).
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                return shift ? Reorder(state, context, TerminalCommand.ReorderUp) : Navigate(state, context, TerminalCommand.NavigateUp);
            case ConsoleKey.DownArrow:
                return shift ? Reorder(state, context, TerminalCommand.ReorderDown) : Navigate(state, context, TerminalCommand.NavigateDown);
            case ConsoleKey.LeftArrow:
                return shift ? MoveByDay(state, context, TerminalCommand.MoveDayBackward) : Navigate(state, context, TerminalCommand.NavigateLeft);
            case ConsoleKey.RightArrow:
                return shift ? MoveByDay(state, context, TerminalCommand.MoveDayForward) : Navigate(state, context, TerminalCommand.NavigateRight);
        }

        // Ctrl+R forces a reconnect; checked before the plain 'r' Routine toggle because the control combo
        // may or may not also carry the 'r' KeyChar depending on the terminal.
        if (key.Key == ConsoleKey.R && (key.Modifiers & ConsoleModifiers.Control) != 0)
            return Raise(state, TerminalCommand.ForceReconnect);

        if ((key.Modifiers & (ConsoleModifiers.Control | ConsoleModifiers.Alt)) != 0)
            return TerminalInputResult.Unchanged(state);

        if (key.Key == ConsoleKey.Spacebar || key.KeyChar == ' ')
            return context.HasFocus ? Raise(state, TerminalCommand.ToggleComplete) : Declined(state, "No task selected.");

        // Letter shortcuts keep the original KeyChar dispatch. A small normalization layer translates
        // standard Russian-layout characters back to the equivalent English shortcut characters.
        return NormalizeShortcutChar(key.KeyChar) switch
        {
            'k' => Navigate(state, context, TerminalCommand.NavigateUp),
            'j' => Navigate(state, context, TerminalCommand.NavigateDown),
            'h' => Navigate(state, context, TerminalCommand.NavigateLeft),
            'l' => Navigate(state, context, TerminalCommand.NavigateRight),
            'K' => Reorder(state, context, TerminalCommand.ReorderUp),
            'J' => Reorder(state, context, TerminalCommand.ReorderDown),
            'H' => MoveByDay(state, context, TerminalCommand.MoveDayBackward),
            'L' => MoveByDay(state, context, TerminalCommand.MoveDayForward),
            'a' => OpenEditor(EditorPurpose.AddWithFocusDate, LineEditorState.Empty, context.FocusDate),
            'A' => OpenEditor(EditorPurpose.AddNoDate, LineEditorState.Empty, fallbackDate: null),
            'e' => context.HasFocus
                ? OpenEditor(EditorPurpose.Edit, LineEditorState.For(context.EditText), fallbackDate: null, context.FocusUid)
                : Declined(state, "No task selected."),
            'm' => context.HasFocus
                ? OpenEditor(EditorPurpose.Move, LineEditorState.For(context.MoveText), fallbackDate: null, context.FocusUid)
                : Declined(state, "No task selected."),
            'd' => context.HasFocus
                ? EnterMode(state, TerminalUiMode.DeleteConfirmation, context.FocusUid)
                : Declined(state, "No task selected."),
            'r' => Raise(state, TerminalCommand.ToggleRoutine),
            'c' => Raise(state, TerminalCommand.ToggleCompletedVisibility),
            '?' => EnterMode(state, TerminalUiMode.Help),
            'q' => Raise(state, TerminalCommand.Quit),
            _ => TerminalInputResult.Unchanged(state),
        };
    }

    private TerminalInputResult ReduceEditing(TerminalInputState state, ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.Enter:
                return Commit(state);
            case ConsoleKey.Escape:
                return Cancelled();
            case ConsoleKey.LeftArrow:
                return EditBuffer(state, state.Editor.MoveLeft());
            case ConsoleKey.RightArrow:
                return EditBuffer(state, state.Editor.MoveRight());
            case ConsoleKey.Home:
                return EditBuffer(state, state.Editor.Home());
            case ConsoleKey.End:
                return EditBuffer(state, state.Editor.End());
            case ConsoleKey.Backspace:
                return EditBuffer(state, state.Editor.Backspace());
            case ConsoleKey.Delete:
                return EditBuffer(state, state.Editor.Delete());
        }

        // Any printable character is text; control combos (Ctrl/Alt) and non-printing keys are ignored so
        // the buffer only ever gains real content.
        if (IsPrintable(key))
            return EditBuffer(state, state.Editor.Insert(key.KeyChar.ToString()));

        return TerminalInputResult.Unchanged(state);
    }

    private static TerminalInputResult ReduceConfirmation(TerminalInputState state, ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Escape)
            return Cancelled();

        return NormalizeShortcutChar(key.KeyChar) switch
        {
            'y' or 'Y' => new TerminalInputResult
            {
                State = TerminalInputState.Normal,
                Command = TerminalCommand.ConfirmDelete,
            },
            'n' or 'N' => Cancelled(),
            _ => TerminalInputResult.Unchanged(state),
        };
    }

    private static TerminalInputResult ReduceHelp(TerminalInputState state, ConsoleKeyInfo key)
    {
        if (NormalizeShortcutChar(key.KeyChar) == '?' || key.Key == ConsoleKey.Escape)
            return new TerminalInputResult { State = TerminalInputState.Normal };

        return TerminalInputResult.Unchanged(state);
    }

    private static TerminalInputResult ReduceResizeRequired(TerminalInputState state, ConsoleKeyInfo key)
    {
        var shortcutChar = NormalizeShortcutChar(key.KeyChar);
        if (shortcutChar == '?')
            return new TerminalInputResult { State = state with { Mode = TerminalUiMode.Help } };
        if (shortcutChar == 'q')
            return new TerminalInputResult { State = state, Command = TerminalCommand.Quit };

        return TerminalInputResult.Unchanged(state);
    }

    private static bool IsPrintable(ConsoleKeyInfo key)
    {
        if ((key.Modifiers & (ConsoleModifiers.Control | ConsoleModifiers.Alt)) != 0)
            return false;

        return key.KeyChar is >= ' ' and not '\u007f';
    }

    private static char NormalizeShortcutChar(char character)
    {
        var normalized = char.ToLowerInvariant(character) switch
        {
            '\u0439' => 'q',
            '\u0446' => 'w',
            '\u0443' => 'e',
            '\u043a' => 'r',
            '\u0435' => 't',
            '\u043d' => 'y',
            '\u0433' => 'u',
            '\u0448' => 'i',
            '\u0449' => 'o',
            '\u0437' => 'p',
            '\u0445' => '[',
            '\u044a' => ']',
            '\u0444' => 'a',
            '\u044b' => 's',
            '\u0432' => 'd',
            '\u0430' => 'f',
            '\u043f' => 'g',
            '\u0440' => 'h',
            '\u043e' => 'j',
            '\u043b' => 'k',
            '\u0434' => 'l',
            '\u0436' => ';',
            '\u044d' => '\'',
            '\u044f' => 'z',
            '\u0447' => 'x',
            '\u0441' => 'c',
            '\u043c' => 'v',
            '\u0438' => 'b',
            '\u0442' => 'n',
            '\u044c' => 'm',
            '\u0431' => ',',
            '\u044e' => '.',
            _ => character,
        };

        return char.IsUpper(character) ? char.ToUpperInvariant(normalized) : normalized;
    }

    private static TerminalInputResult Navigate(TerminalInputState state, TerminalInputContext context, TerminalCommand command)
    {
        // With no visible task there is nothing to move onto, so navigation is a silent no-op rather than
        // a status message.
        return context.HasFocus ? Raise(state, command) : TerminalInputResult.Unchanged(state);
    }

    private static TerminalInputResult Reorder(TerminalInputState state, TerminalInputContext context, TerminalCommand command)
    {
        return context.HasFocus ? Raise(state, command) : Declined(state, "No task selected.");
    }

    private static TerminalInputResult MoveByDay(TerminalInputState state, TerminalInputContext context, TerminalCommand command)
    {
        if (!context.HasFocus)
            return Declined(state, "No task selected.");
        if (!context.FocusHasDate)
            return Declined(state, "Set a date before moving a No Date task by day.");

        return Raise(state, command);
    }

    private TerminalInputResult OpenEditor(
        EditorPurpose purpose, LineEditorState editor, DateOnly? fallbackDate, string? targetUid = null)
    {
        return new TerminalInputResult
        {
            State = new TerminalInputState
            {
                Mode = TerminalUiMode.Editor,
                Purpose = purpose,
                Editor = editor,
                TargetUid = targetUid,
                Feedback = ComputeFeedback(purpose, editor.Text, fallbackDate),
                FallbackDate = fallbackDate,
            },
        };
    }

    private TerminalInputResult EditBuffer(TerminalInputState state, LineEditorState editor)
    {
        return TerminalInputResult.Unchanged(state with
        {
            Editor = editor,
            Feedback = ComputeFeedback(state.Purpose, editor.Text, state.FallbackDate),
        });
    }

    private static TerminalInputResult Commit(TerminalInputState state)
    {
        var text = state.Editor.Text;

        // An empty add/edit commit is treated as a cancel: it can neither create nor rename a task. Login
        // (validated by the application) and Move (where an empty value means "move to No Date") still
        // submit their commands so those explicit actions remain reachable from the editor.
        if (state.Purpose is not (EditorPurpose.Login or EditorPurpose.Move) && string.IsNullOrWhiteSpace(text))
            return Cancelled();

        var command = state.Purpose switch
        {
            EditorPurpose.AddWithFocusDate => TerminalCommand.SubmitAddWithFocusDate,
            EditorPurpose.AddNoDate => TerminalCommand.SubmitAddNoDate,
            EditorPurpose.Edit => TerminalCommand.SubmitEdit,
            EditorPurpose.Move => TerminalCommand.SubmitMove,
            EditorPurpose.Login => TerminalCommand.SubmitLogin,
            EditorPurpose.None => TerminalCommand.None,
            _ => TerminalCommand.None,
        };

        return new TerminalInputResult
        {
            State = TerminalInputState.Normal,
            Command = command,
            CommittedText = text,
        };
    }

    private static TerminalInputResult Cancelled()
    {
        return new TerminalInputResult { State = TerminalInputState.Normal };
    }

    private static TerminalInputResult Raise(TerminalInputState state, TerminalCommand command)
    {
        return new TerminalInputResult { State = state, Command = command };
    }

    private static TerminalInputResult Declined(TerminalInputState state, string message)
    {
        return new TerminalInputResult { State = state, StatusMessage = message };
    }

    private static TerminalInputResult EnterMode(
        TerminalInputState state, TerminalUiMode mode, string? targetUid = null)
    {
        return new TerminalInputResult
        {
            State = state with
            {
                Mode = mode,
                Purpose = EditorPurpose.None,
                Editor = LineEditorState.Empty,
                TargetUid = targetUid,
                Feedback = null,
                FallbackDate = null,
            },
        };
    }

    // Live shared-parser feedback for the add/edit editors only: it parses the current buffer with the
    // same TaskTextParser the backend uses and renders the parsed task back through the editor formatter,
    // applying an AddWithFocusDate's inherited date so a preview reflects the date the task will actually
    // get. A range shows its task count; a parse error shows the parser's message. Move and login editors
    // return no feedback (Move commits a bare date and login is masked).
    private string? ComputeFeedback(EditorPurpose purpose, string text, DateOnly? fallbackDate)
    {
        if (purpose is not (EditorPurpose.AddWithFocusDate or EditorPurpose.AddNoDate or EditorPurpose.Edit))
            return null;

        if (string.IsNullOrWhiteSpace(text))
            return null;

        try
        {
            var parsed = parser.Parse(text);
            if (parsed.Count != 1)
                return $"{parsed.Count} tasks";

            var single = parsed[0];
            if (single.Date is null && fallbackDate is { } date)
                single = single with { Date = date };

            return formatter.Format(single);
        }
        catch (TaskTextParseException exception)
        {
            return exception.Message;
        }
    }
}
