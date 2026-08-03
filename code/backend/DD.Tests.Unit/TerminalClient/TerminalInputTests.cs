using DD.Shared.TaskText;
using DD.TerminalClient.Details.Ui;
using DD.TerminalClient.Domain.Abstractions;
using DD.TerminalClient.Domain.Editing;
using DD.TerminalClient.Domain.Input;
using DD.TerminalClient.Domain.Time;
using Spectre.Console.Testing;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Exercises the keyboard input layer: the pure TerminalInputReducer state machine (the exact normal-mode
// keymap and its aliases, the add/edit/move/login editors, delete-confirmation, help and resize-required
// modes, invalid-context declines, live parser feedback, commit and cancel), the LineEditorState buffer
// operations, and the TerminalInputReader adapter plus the IKeyInputSource cancellation contract. Every
// key is a constructed ConsoleKeyInfo and every awaited read carries an explicit timeout so a run can
// never hang on real input.
public sealed class TerminalInputTests
{
    private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(5);

    private static readonly TerminalInputContext NoFocus = new() { HasFocus = false };

    // ----- Normal-mode navigation -----
    [Fact]
    public void Normal_Arrows_Navigate()
    {
        var reducer = Reducer();
        var context = Focused();

        Assert.Equal(TerminalCommand.NavigateUp, reducer.Reduce(TerminalInputState.Normal, Arrow(ConsoleKey.UpArrow), context).Command);
        Assert.Equal(TerminalCommand.NavigateDown, reducer.Reduce(TerminalInputState.Normal, Arrow(ConsoleKey.DownArrow), context).Command);
        Assert.Equal(TerminalCommand.NavigateLeft, reducer.Reduce(TerminalInputState.Normal, Arrow(ConsoleKey.LeftArrow), context).Command);
        Assert.Equal(TerminalCommand.NavigateRight, reducer.Reduce(TerminalInputState.Normal, Arrow(ConsoleKey.RightArrow), context).Command);
    }

    [Fact]
    public void Normal_HjklAliases_Navigate()
    {
        var reducer = Reducer();
        var context = Focused();

        Assert.Equal(TerminalCommand.NavigateUp, reducer.Reduce(TerminalInputState.Normal, Key('k'), context).Command);
        Assert.Equal(TerminalCommand.NavigateDown, reducer.Reduce(TerminalInputState.Normal, Key('j'), context).Command);
        Assert.Equal(TerminalCommand.NavigateLeft, reducer.Reduce(TerminalInputState.Normal, Key('h'), context).Command);
        Assert.Equal(TerminalCommand.NavigateRight, reducer.Reduce(TerminalInputState.Normal, Key('l'), context).Command);
    }

    [Fact]
    public void Normal_NavigationWithoutFocus_IsSilentNoOp()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Arrow(ConsoleKey.DownArrow), NoFocus);

        Assert.Equal(TerminalCommand.None, result.Command);
        Assert.Null(result.StatusMessage);
        Assert.Same(TerminalInputState.Normal, result.State);
    }

    // ----- Normal-mode reorder and day-move (Shift + uppercase aliases) -----
    [Fact]
    public void Normal_ShiftArrows_ReorderAndMoveByDay()
    {
        var reducer = Reducer();
        var context = Focused(hasDate: true);

        Assert.Equal(TerminalCommand.ReorderUp, reducer.Reduce(TerminalInputState.Normal, Arrow(ConsoleKey.UpArrow, shift: true), context).Command);
        Assert.Equal(TerminalCommand.ReorderDown, reducer.Reduce(TerminalInputState.Normal, Arrow(ConsoleKey.DownArrow, shift: true), context).Command);
        Assert.Equal(TerminalCommand.MoveDayBackward, reducer.Reduce(TerminalInputState.Normal, Arrow(ConsoleKey.LeftArrow, shift: true), context).Command);
        Assert.Equal(TerminalCommand.MoveDayForward, reducer.Reduce(TerminalInputState.Normal, Arrow(ConsoleKey.RightArrow, shift: true), context).Command);
    }

    [Fact]
    public void Normal_UppercaseAliases_ReorderAndMoveByDay()
    {
        var reducer = Reducer();
        var context = Focused(hasDate: true);

        Assert.Equal(TerminalCommand.ReorderUp, reducer.Reduce(TerminalInputState.Normal, Key('K'), context).Command);
        Assert.Equal(TerminalCommand.ReorderDown, reducer.Reduce(TerminalInputState.Normal, Key('J'), context).Command);
        Assert.Equal(TerminalCommand.MoveDayBackward, reducer.Reduce(TerminalInputState.Normal, Key('H'), context).Command);
        Assert.Equal(TerminalCommand.MoveDayForward, reducer.Reduce(TerminalInputState.Normal, Key('L'), context).Command);
    }

    [Fact]
    public void Normal_ReorderWithoutFocus_IsDeclinedWithStatus()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Key('K'), NoFocus);

        Assert.Equal(TerminalCommand.None, result.Command);
        Assert.False(string.IsNullOrEmpty(result.StatusMessage));
    }

    [Fact]
    public void Normal_MoveByDayWithoutFocus_IsDeclinedWithStatus()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Key('H'), NoFocus);

        Assert.Equal(TerminalCommand.None, result.Command);
        Assert.False(string.IsNullOrEmpty(result.StatusMessage));
    }

    [Fact]
    public void Normal_MoveByDayWithoutDate_IsDeclinedWithStatus()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Key('L'), Focused(hasDate: false));

        Assert.Equal(TerminalCommand.None, result.Command);
        Assert.False(string.IsNullOrEmpty(result.StatusMessage));
    }

    // ----- Normal-mode single-key commands -----
    [Fact]
    public void Normal_Space_TogglesCompleteWhenFocused()
    {
        var reducer = Reducer();

        Assert.Equal(TerminalCommand.ToggleComplete, reducer.Reduce(TerminalInputState.Normal, Space(), Focused()).Command);
    }

    [Fact]
    public void Normal_SpaceWithoutFocus_IsDeclinedWithStatus()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Space(), NoFocus);

        Assert.Equal(TerminalCommand.None, result.Command);
        Assert.False(string.IsNullOrEmpty(result.StatusMessage));
    }

    [Fact]
    public void Normal_ToggleCompletedVisibility_IsGlobal()
    {
        var reducer = Reducer();

        Assert.Equal(TerminalCommand.ToggleCompletedVisibility, reducer.Reduce(TerminalInputState.Normal, Key('c'), NoFocus).Command);
    }

    [Fact]
    public void Normal_CtrlR_ForcesReconnect()
    {
        var reducer = Reducer();

        Assert.Equal(TerminalCommand.ForceReconnect, reducer.Reduce(TerminalInputState.Normal, Ctrl(ConsoleKey.R, 'r'), NoFocus).Command);
    }

    [Fact]
    public void Normal_RoutineToggle_RequiresDatedFocus()
    {
        var reducer = Reducer();

        Assert.Equal(TerminalCommand.ToggleRoutine, reducer.Reduce(TerminalInputState.Normal, Key('r'), Focused(hasDate: true)).Command);
        Assert.Equal(TerminalCommand.None, reducer.Reduce(TerminalInputState.Normal, Key('r'), Focused(hasDate: false)).Command);
        Assert.False(string.IsNullOrEmpty(reducer.Reduce(TerminalInputState.Normal, Key('r'), NoFocus).StatusMessage));
    }

    [Fact]
    public void Normal_Quit_RaisesQuit()
    {
        var reducer = Reducer();

        Assert.Equal(TerminalCommand.Quit, reducer.Reduce(TerminalInputState.Normal, Key('q'), NoFocus).Command);
    }

    // ----- Normal-mode mode transitions -----
    [Fact]
    public void Normal_AddLower_OpensEditorWithFocusDate()
    {
        var reducer = Reducer();
        var focusDate = new DateOnly(2024, 6, 12);

        var result = reducer.Reduce(TerminalInputState.Normal, Key('a'), Focused(focusDate: focusDate));

        Assert.Equal(TerminalUiMode.Editor, result.State.Mode);
        Assert.Equal(EditorPurpose.AddWithFocusDate, result.State.Purpose);
        Assert.Equal(focusDate, result.State.FallbackDate);
        Assert.Equal(string.Empty, result.State.Editor.Text);
        Assert.Equal(TerminalCommand.None, result.Command);
    }

    [Fact]
    public void Normal_AddUpper_OpensNoDateEditor()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Key('A'), Focused(focusDate: new DateOnly(2024, 6, 12)));

        Assert.Equal(TerminalUiMode.Editor, result.State.Mode);
        Assert.Equal(EditorPurpose.AddNoDate, result.State.Purpose);
        Assert.Null(result.State.FallbackDate);
    }

    [Fact]
    public void Normal_Edit_OpensEditorPrefilledWithCursorAtEnd()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Key('e'), Focused(edit: "1200 Lunch"));

        Assert.Equal(TerminalUiMode.Editor, result.State.Mode);
        Assert.Equal(EditorPurpose.Edit, result.State.Purpose);
        Assert.Equal("1200 Lunch", result.State.Editor.Text);
        Assert.Equal("1200 Lunch".Length, result.State.Editor.Cursor);
    }

    [Fact]
    public void Normal_EditWithoutFocus_IsDeclinedWithStatus()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Key('e'), NoFocus);

        Assert.Equal(TerminalUiMode.Normal, result.State.Mode);
        Assert.False(string.IsNullOrEmpty(result.StatusMessage));
    }

    [Fact]
    public void Normal_Move_OpensEditorPrefilledFromMoveText()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Key('m'), Focused(move: "0612"));

        Assert.Equal(TerminalUiMode.Editor, result.State.Mode);
        Assert.Equal(EditorPurpose.Move, result.State.Purpose);
        Assert.Equal("0612", result.State.Editor.Text);
    }

    [Fact]
    public void Normal_MoveWithoutFocus_IsDeclinedWithStatus()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Key('m'), NoFocus);

        Assert.Equal(TerminalUiMode.Normal, result.State.Mode);
        Assert.False(string.IsNullOrEmpty(result.StatusMessage));
    }

    [Fact]
    public void Normal_Delete_EntersConfirmationWhenFocused()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Key('d'), Focused());

        Assert.Equal(TerminalUiMode.DeleteConfirmation, result.State.Mode);
        Assert.Equal(TerminalCommand.None, result.Command);
    }

    [Fact]
    public void Normal_DeleteWithoutFocus_IsDeclinedWithStatus()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Key('d'), NoFocus);

        Assert.Equal(TerminalUiMode.Normal, result.State.Mode);
        Assert.False(string.IsNullOrEmpty(result.StatusMessage));
    }

    [Fact]
    public void Normal_Help_EntersHelpMode()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Key('?'), NoFocus);

        Assert.Equal(TerminalUiMode.Help, result.State.Mode);
    }

    // ----- Editor operations -----
    [Fact]
    public void Editor_PrintableKeys_InsertText()
    {
        var reducer = Reducer();
        var opened = reducer.Reduce(TerminalInputState.Normal, Key('A'), NoFocus).State;

        var result = Type(reducer, opened, "Hi");

        Assert.Equal("Hi", result.State.Editor.Text);
        Assert.Equal(2, result.State.Editor.Cursor);
        Assert.Equal(TerminalCommand.None, result.Command);
    }

    [Fact]
    public void Editor_CursorMovementAndHomeEnd_RepositionCursor()
    {
        var reducer = Reducer();
        var opened = Type(reducer, reducer.Reduce(TerminalInputState.Normal, Key('A'), NoFocus).State, "abc").State;

        var afterLeft = reducer.Reduce(opened, Special(ConsoleKey.LeftArrow), NoFocus).State;
        Assert.Equal(2, afterLeft.Editor.Cursor);

        var afterHome = reducer.Reduce(afterLeft, Special(ConsoleKey.Home), NoFocus).State;
        Assert.Equal(0, afterHome.Editor.Cursor);

        var afterRight = reducer.Reduce(afterHome, Special(ConsoleKey.RightArrow), NoFocus).State;
        Assert.Equal(1, afterRight.Editor.Cursor);

        var afterEnd = reducer.Reduce(afterRight, Special(ConsoleKey.End), NoFocus).State;
        Assert.Equal(3, afterEnd.Editor.Cursor);
    }

    [Fact]
    public void Editor_BackspaceAndDelete_EditAroundCursor()
    {
        var reducer = Reducer();
        var opened = Type(reducer, reducer.Reduce(TerminalInputState.Normal, Key('A'), NoFocus).State, "abc").State;

        var afterBackspace = reducer.Reduce(opened, Special(ConsoleKey.Backspace), NoFocus).State;
        Assert.Equal("ab", afterBackspace.Editor.Text);
        Assert.Equal(2, afterBackspace.Editor.Cursor);

        var atStart = reducer.Reduce(afterBackspace, Special(ConsoleKey.Home), NoFocus).State;
        var afterDelete = reducer.Reduce(atStart, Special(ConsoleKey.Delete), NoFocus).State;
        Assert.Equal("b", afterDelete.Editor.Text);
        Assert.Equal(0, afterDelete.Editor.Cursor);
    }

    [Fact]
    public void Editor_PrintableCommandKeys_DoNotTriggerNormalCommands()
    {
        var reducer = Reducer();
        var opened = reducer.Reduce(TerminalInputState.Normal, Key('A'), NoFocus).State;

        var result = Type(reducer, opened, "qad");

        Assert.Equal("qad", result.State.Editor.Text);
        Assert.Equal(TerminalUiMode.Editor, result.State.Mode);
        Assert.Equal(TerminalCommand.None, result.Command);
    }

    [Fact]
    public void Editor_EnterOnAddNoDate_CommitsWithText()
    {
        var reducer = Reducer();
        var typed = Type(reducer, reducer.Reduce(TerminalInputState.Normal, Key('A'), NoFocus).State, "Buy milk").State;

        var result = reducer.Reduce(typed, Special(ConsoleKey.Enter, '\r'), NoFocus);

        Assert.Equal(TerminalCommand.SubmitAddNoDate, result.Command);
        Assert.Equal("Buy milk", result.CommittedText);
        Assert.Equal(TerminalUiMode.Normal, result.State.Mode);
    }

    [Fact]
    public void Editor_EnterOnEdit_CommitsEdit()
    {
        var reducer = Reducer();
        var typed = Type(reducer, reducer.Reduce(TerminalInputState.Normal, Key('e'), Focused(edit: "Old")).State, "!").State;

        var result = reducer.Reduce(typed, Special(ConsoleKey.Enter, '\r'), NoFocus);

        Assert.Equal(TerminalCommand.SubmitEdit, result.Command);
        Assert.Equal("Old!", result.CommittedText);
    }

    [Fact]
    public void Editor_EnterOnMove_CommitsMove()
    {
        var reducer = Reducer();
        var opened = reducer.Reduce(TerminalInputState.Normal, Key('m'), Focused(move: "0612")).State;

        var result = reducer.Reduce(opened, Special(ConsoleKey.Enter, '\r'), NoFocus);

        Assert.Equal(TerminalCommand.SubmitMove, result.Command);
        Assert.Equal("0612", result.CommittedText);
    }

    [Fact]
    public void Editor_EnterOnEmptyMove_SubmitsMoveSoTheTaskCanGoToNoDate()
    {
        var reducer = Reducer();
        var opened = reducer.Reduce(TerminalInputState.Normal, Key('m'), Focused()).State;

        var result = reducer.Reduce(opened, Special(ConsoleKey.Enter, '\r'), NoFocus);

        Assert.Equal(TerminalCommand.SubmitMove, result.Command);
        Assert.Equal(string.Empty, result.CommittedText);
        Assert.Equal(TerminalUiMode.Normal, result.State.Mode);
    }

    [Fact]
    public void Editor_EnterOnEmptyBuffer_CancelsInsteadOfCommitting()
    {
        var reducer = Reducer();
        var opened = reducer.Reduce(TerminalInputState.Normal, Key('A'), NoFocus).State;

        var result = reducer.Reduce(opened, Special(ConsoleKey.Enter, '\r'), NoFocus);

        Assert.Equal(TerminalCommand.None, result.Command);
        Assert.Equal(TerminalUiMode.Normal, result.State.Mode);
    }

    [Fact]
    public void Editor_Escape_CancelsToNormal()
    {
        var reducer = Reducer();
        var typed = Type(reducer, reducer.Reduce(TerminalInputState.Normal, Key('A'), NoFocus).State, "draft").State;

        var result = reducer.Reduce(typed, Special(ConsoleKey.Escape, '\u001b'), NoFocus);

        Assert.Equal(TerminalCommand.None, result.Command);
        Assert.Equal(TerminalUiMode.Normal, result.State.Mode);
        Assert.Equal(string.Empty, result.State.Editor.Text);
    }

    // ----- Live parser feedback -----
    [Fact]
    public void Editor_AddNoDate_FeedbackPreviewsParsedTitle()
    {
        var reducer = Reducer();

        var result = Type(reducer, reducer.Reduce(TerminalInputState.Normal, Key('A'), NoFocus).State, "Buy milk");

        Assert.Equal("Buy milk", result.State.Feedback);
    }

    [Fact]
    public void Editor_AddWithFocusDate_FeedbackAppliesInheritedDate()
    {
        var reducer = Reducer();
        var opened = reducer.Reduce(TerminalInputState.Normal, Key('a'), Focused(focusDate: new DateOnly(2024, 6, 12))).State;

        var result = Type(reducer, opened, "Lunch");

        Assert.Contains("0612", result.State.Feedback, StringComparison.Ordinal);
        Assert.Contains("Lunch", result.State.Feedback, StringComparison.Ordinal);
    }

    [Fact]
    public void Editor_ParseError_FeedbackShowsMessage()
    {
        var reducer = Reducer();

        var result = Type(reducer, reducer.Reduce(TerminalInputState.Normal, Key('A'), NoFocus).State, "1350 x");

        Assert.Contains("invalid", result.State.Feedback, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Editor_DateRange_FeedbackShowsTaskCount()
    {
        var reducer = Reducer();

        var result = Type(reducer, reducer.Reduce(TerminalInputState.Normal, Key('A'), NoFocus).State, "0620-0622 Trip");

        Assert.Equal("3 tasks", result.State.Feedback);
    }

    [Fact]
    public void Editor_EmptyBuffer_HasNoFeedback()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.Normal, Key('A'), NoFocus);

        Assert.Null(result.State.Feedback);
    }

    // ----- Masked login -----
    [Fact]
    public void Login_KeepsBufferButProducesNoFeedback()
    {
        var reducer = Reducer();

        var result = Type(reducer, TerminalInputState.BeginLogin(), "secret");

        Assert.Equal(TerminalUiMode.MaskedLogin, result.State.Mode);
        Assert.Equal("secret", result.State.Editor.Text);
        Assert.Null(result.State.Feedback);
    }

    [Fact]
    public void Login_Enter_SubmitsCredential()
    {
        var reducer = Reducer();
        var typed = Type(reducer, TerminalInputState.BeginLogin(), "secret").State;

        var result = reducer.Reduce(typed, Special(ConsoleKey.Enter, '\r'), NoFocus);

        Assert.Equal(TerminalCommand.SubmitLogin, result.Command);
        Assert.Equal("secret", result.CommittedText);
        Assert.Equal(TerminalUiMode.Normal, result.State.Mode);
    }

    [Fact]
    public void Login_EnterOnEmptyBuffer_StillSubmits()
    {
        var reducer = Reducer();

        var result = reducer.Reduce(TerminalInputState.BeginLogin(), Special(ConsoleKey.Enter, '\r'), NoFocus);

        Assert.Equal(TerminalCommand.SubmitLogin, result.Command);
        Assert.Equal(string.Empty, result.CommittedText);
    }

    [Fact]
    public void Login_Escape_CancelsToNormal()
    {
        var reducer = Reducer();
        var typed = Type(reducer, TerminalInputState.BeginLogin(), "secret").State;

        var result = reducer.Reduce(typed, Special(ConsoleKey.Escape, '\u001b'), NoFocus);

        Assert.Equal(TerminalCommand.None, result.Command);
        Assert.Equal(TerminalUiMode.Normal, result.State.Mode);
    }

    // ----- Delete confirmation -----
    [Fact]
    public void Confirmation_Yes_ConfirmsDelete()
    {
        var reducer = Reducer();
        var confirming = reducer.Reduce(TerminalInputState.Normal, Key('d'), Focused()).State;

        var result = reducer.Reduce(confirming, Key('y'), NoFocus);

        Assert.Equal(TerminalCommand.ConfirmDelete, result.Command);
        Assert.Equal(TerminalUiMode.Normal, result.State.Mode);
    }

    [Fact]
    public void Confirmation_No_CancelsWithoutDeleting()
    {
        var reducer = Reducer();
        var confirming = reducer.Reduce(TerminalInputState.Normal, Key('d'), Focused()).State;

        var result = reducer.Reduce(confirming, Key('n'), NoFocus);

        Assert.Equal(TerminalCommand.None, result.Command);
        Assert.Equal(TerminalUiMode.Normal, result.State.Mode);
    }

    [Fact]
    public void Confirmation_Escape_Cancels()
    {
        var reducer = Reducer();
        var confirming = reducer.Reduce(TerminalInputState.Normal, Key('d'), Focused()).State;

        var result = reducer.Reduce(confirming, Special(ConsoleKey.Escape, '\u001b'), NoFocus);

        Assert.Equal(TerminalCommand.None, result.Command);
        Assert.Equal(TerminalUiMode.Normal, result.State.Mode);
    }

    [Fact]
    public void Confirmation_UnrelatedKey_IsIgnored()
    {
        var reducer = Reducer();
        var confirming = reducer.Reduce(TerminalInputState.Normal, Key('d'), Focused()).State;

        var result = reducer.Reduce(confirming, Key('x'), NoFocus);

        Assert.Equal(TerminalCommand.None, result.Command);
        Assert.Equal(TerminalUiMode.DeleteConfirmation, result.State.Mode);
    }

    // ----- Help mode -----
    [Fact]
    public void Help_QuestionMark_Closes()
    {
        var reducer = Reducer();
        var helping = reducer.Reduce(TerminalInputState.Normal, Key('?'), NoFocus).State;

        Assert.Equal(TerminalUiMode.Normal, reducer.Reduce(helping, Key('?'), NoFocus).State.Mode);
    }

    [Fact]
    public void Help_Escape_Closes()
    {
        var reducer = Reducer();
        var helping = reducer.Reduce(TerminalInputState.Normal, Key('?'), NoFocus).State;

        Assert.Equal(TerminalUiMode.Normal, reducer.Reduce(helping, Special(ConsoleKey.Escape, '\u001b'), NoFocus).State.Mode);
    }

    [Fact]
    public void Help_OtherKey_StaysOpen()
    {
        var reducer = Reducer();
        var helping = reducer.Reduce(TerminalInputState.Normal, Key('?'), NoFocus).State;

        var result = reducer.Reduce(helping, Key('q'), NoFocus);

        Assert.Equal(TerminalUiMode.Help, result.State.Mode);
        Assert.Equal(TerminalCommand.None, result.Command);
    }

    // ----- Resize-required mode -----
    [Fact]
    public void ResizeRequired_QuestionMark_OpensHelp()
    {
        var reducer = Reducer();
        var resize = TerminalInputState.Normal with { Mode = TerminalUiMode.ResizeRequired };

        Assert.Equal(TerminalUiMode.Help, reducer.Reduce(resize, Key('?'), NoFocus).State.Mode);
    }

    [Fact]
    public void ResizeRequired_Quit_RaisesQuit()
    {
        var reducer = Reducer();
        var resize = TerminalInputState.Normal with { Mode = TerminalUiMode.ResizeRequired };

        Assert.Equal(TerminalCommand.Quit, reducer.Reduce(resize, Key('q'), NoFocus).Command);
    }

    [Fact]
    public void ResizeRequired_OtherKey_IsIgnoredWhileSyncing()
    {
        var reducer = Reducer();
        var resize = TerminalInputState.Normal with { Mode = TerminalUiMode.ResizeRequired };

        var result = reducer.Reduce(resize, Key('a'), NoFocus);

        Assert.Equal(TerminalCommand.None, result.Command);
        Assert.Equal(TerminalUiMode.ResizeRequired, result.State.Mode);
    }

    // ----- Argument guards -----
    [Fact]
    public void Reduce_NullState_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Reducer().Reduce(null!, Key('q'), NoFocus));
    }

    [Fact]
    public void Reduce_NullContext_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Reducer().Reduce(TerminalInputState.Normal, Key('q'), null!));
    }

    // ----- LineEditorState buffer operations -----
    [Fact]
    public void LineEditor_Insert_AddsAtCursor()
    {
        var editor = LineEditorState.For("ac").MoveLeft().Insert("b");

        Assert.Equal("abc", editor.Text);
        Assert.Equal(2, editor.Cursor);
    }

    [Fact]
    public void LineEditor_InsertChunk_SupportsPaste()
    {
        var editor = LineEditorState.Empty.Insert("pasted text");

        Assert.Equal("pasted text", editor.Text);
        Assert.Equal("pasted text".Length, editor.Cursor);
    }

    [Fact]
    public void LineEditor_BackspaceAtStart_IsNoOp()
    {
        var editor = LineEditorState.For("abc").Home().Backspace();

        Assert.Equal("abc", editor.Text);
        Assert.Equal(0, editor.Cursor);
    }

    [Fact]
    public void LineEditor_DeleteAtEnd_IsNoOp()
    {
        var editor = LineEditorState.For("abc").Delete();

        Assert.Equal("abc", editor.Text);
        Assert.Equal(3, editor.Cursor);
    }

    [Fact]
    public void LineEditor_MovementIsClamped()
    {
        var editor = LineEditorState.For("ab");

        Assert.Equal(2, editor.MoveRight().Cursor);
        Assert.Equal(0, editor.Home().MoveLeft().Cursor);
    }

    [Theory]
    [InlineData(-5, "Xabc")]
    [InlineData(99, "abcX")]
    public void LineEditor_OutOfRangeCursor_ClampsOnInsert(int cursor, string expected)
    {
        var editor = new LineEditorState { Text = "abc", Cursor = cursor };

        var result = editor.Insert("X");

        Assert.Equal(expected, result.Text);
        Assert.InRange(result.Cursor, 0, 4);
    }

    // ----- TerminalInputReader adapter and IKeyInputSource cancellation contract -----
    [Fact]
    public async Task Reader_ReturnsPushedKey()
    {
        var console = new TestConsole();
        console.Input.PushKey(new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));
        var reader = new TerminalInputReader(console);

        var key = await reader.ReadKeyAsync(CancellationToken.None).WaitAsync(WaitTimeout);

        Assert.True(key.HasValue);
        Assert.Equal('x', key.Value.KeyChar);
    }

    [Fact]
    public async Task Reader_ReturnsKeysInPushOrder()
    {
        var console = new TestConsole();
        console.Input.PushKey(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
        console.Input.PushKey(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false));
        var reader = new TerminalInputReader(console);

        var first = await reader.ReadKeyAsync(CancellationToken.None).WaitAsync(WaitTimeout);
        var second = await reader.ReadKeyAsync(CancellationToken.None).WaitAsync(WaitTimeout);

        Assert.Equal('a', first.Value.KeyChar);
        Assert.Equal(ConsoleKey.UpArrow, second.Value.Key);
    }

    [Fact]
    public async Task KeyInputSource_HonoursCancellation()
    {
        IKeyInputSource source = new BlockingKeySource();
        using var cts = new CancellationTokenSource();

        var reading = source.ReadKeyAsync(cts.Token);
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => reading.WaitAsync(WaitTimeout));
    }

    // ----- Helpers -----
    private static TerminalInputReducer Reducer()
    {
        var dates = new FixedDates();
        return new TerminalInputReducer(new TaskTextParser(dates), new TaskTextFormatter(dates));
    }

    private static TerminalInputContext Focused(
        bool hasDate = true,
        string edit = "Sample",
        string move = "",
        DateOnly? focusDate = null)
    {
        return new TerminalInputContext
        {
            HasFocus = true,
            FocusHasDate = hasDate,
            EditText = edit,
            MoveText = move,
            FocusDate = focusDate,
        };
    }

    private static TerminalInputResult Type(TerminalInputReducer reducer, TerminalInputState start, string text)
    {
        var result = TerminalInputResult.Unchanged(start);
        foreach (var character in text)
        {
            result = reducer.Reduce(result.State, Key(character), NoFocus);
        }

        return result;
    }

    private static ConsoleKeyInfo Key(char character)
    {
        var key = char.IsLetter(character) ? (ConsoleKey)char.ToUpperInvariant(character) : 0;
        return new ConsoleKeyInfo(character, key, shift: char.IsUpper(character), alt: false, control: false);
    }

    private static ConsoleKeyInfo Arrow(ConsoleKey key, bool shift = false)
    {
        return new ConsoleKeyInfo('\0', key, shift, alt: false, control: false);
    }

    private static ConsoleKeyInfo Special(ConsoleKey key, char character = '\0')
    {
        return new ConsoleKeyInfo(character, key, shift: false, alt: false, control: false);
    }

    private static ConsoleKeyInfo Ctrl(ConsoleKey key, char character)
    {
        return new ConsoleKeyInfo(character, key, shift: false, alt: false, control: true);
    }

    private static ConsoleKeyInfo Space()
    {
        return new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, shift: false, alt: false, control: false);
    }

    private sealed class FixedDates : ILocalDateProvider, ITaskTextDateProvider
    {
        public DateOnly Today => new(2024, 6, 10);
    }

    private sealed class BlockingKeySource : IKeyInputSource
    {
        public async Task<ConsoleKeyInfo?> ReadKeyAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
            return null;
        }
    }
}
