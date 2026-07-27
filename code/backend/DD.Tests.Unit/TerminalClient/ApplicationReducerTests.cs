using DD.Shared.TaskText;
using DD.TerminalClient.Domain.Application;
using DD.TerminalClient.Domain.Editing;
using DD.TerminalClient.Domain.Input;
using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Navigation;
using DD.TerminalClient.Domain.Overview;
using DD.TerminalClient.Domain.Time;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Exercises the ApplicationReducer command wiring in the Ready phase: every ExecuteCommand branch that
// turns a committed editor value or normal-mode key into a task mutation, the resulting cache/effect,
// and the parse-error decline path. The pure mutation, navigation and input services are covered on
// their own elsewhere; these tests lock the reducer that stitches them together end to end.
public sealed class ApplicationReducerTests
{
    private static readonly DateOnly Monday = new(2024, 6, 10);

    [Fact]
    public void Add_WithFocusDate_CreatesTaskOnFocusedDateAndEnqueuesSave()
    {
        var reducer = NewReducer();
        var state = Ready(reducer, Task("a", Monday, "Existing"));

        var result = Feed(reducer, state, Then(Key('a'), Typed("Buy milk"), Enter()));

        var created = result.State.Cache.Single(task => task.Title == "Buy milk");
        Assert.Equal(Monday, created.Date);
        Assert.Contains(result.Effects, effect => effect.Kind == ApplicationEffectKind.EnqueueTaskChanges);
    }

    [Fact]
    public void AddNoDate_CreatesTaskWithoutDate()
    {
        var reducer = NewReducer();
        var state = Ready(reducer, Task("a", Monday, "Existing"));

        var result = Feed(reducer, state, Then(Key('A'), Typed("Later"), Enter()));

        var created = result.State.Cache.Single(task => task.Title == "Later");
        Assert.Null(created.Date);
    }

    [Fact]
    public void Edit_ReplacesFocusedTaskTitle()
    {
        var reducer = NewReducer();
        var state = Ready(reducer, Task("a", date: null, title: "Old"));

        var result = Feed(reducer, state, Then(Key('e'), Typed("!"), Enter()));

        Assert.Equal("Old!", result.State.Cache.Single(task => task.Uid == "a").Title);
    }

    [Fact]
    public void Move_ToExplicitDate_SetsFocusedTaskDate()
    {
        var reducer = NewReducer();
        var state = Ready(reducer, Task("a", Monday, "A"));

        var result = Feed(reducer, state, Then(Key('m'), Typed("0612"), Enter()));

        Assert.Equal(new DateOnly(2024, 6, 12), result.State.Cache.Single(task => task.Uid == "a").Date);
    }

    [Fact]
    public void Move_EmptyCommit_MovesFocusedTaskToNoDate()
    {
        var reducer = NewReducer();
        var state = Ready(reducer, Task("a", Monday, "A"));

        var result = Feed(reducer, state, Then(Key('m'), Enter()));

        Assert.Null(result.State.Cache.Single(task => task.Uid == "a").Date);
    }

    [Fact]
    public void Reorder_Down_SwapsFocusedTaskBelowItsVisibleNeighbor()
    {
        var reducer = NewReducer();
        var state = Ready(reducer, Task("a", Monday, "A", order: 1), Task("b", Monday, "B", order: 2));
        state = FocusOn(state, "a");

        var result = Feed(reducer, state, Then(ShiftArrow(ConsoleKey.DownArrow)));

        var a = result.State.Cache.Single(task => task.Uid == "a");
        var b = result.State.Cache.Single(task => task.Uid == "b");
        Assert.True(a.Order > b.Order, "the focused task should be renumbered below its neighbor");
        Assert.Contains(result.Effects, effect => effect.Kind == ApplicationEffectKind.EnqueueTaskChanges);
    }

    [Fact]
    public void MoveByDay_ShiftsFocusedTaskDateForward()
    {
        var reducer = NewReducer();
        var state = Ready(reducer, Task("a", Monday, "A"));

        var result = Feed(reducer, state, Then(ShiftArrow(ConsoleKey.RightArrow)));

        Assert.Equal(Monday.AddDays(1), result.State.Cache.Single(task => task.Uid == "a").Date);
    }

    [Fact]
    public void ToggleComplete_FlipsFocusedTaskCompletion()
    {
        var reducer = NewReducer();
        var state = Ready(reducer, Task("a", Monday, "A"));

        var result = Feed(reducer, state, Then(Space()));

        Assert.True(result.State.Cache.Single(task => task.Uid == "a").Completed);
    }

    [Fact]
    public void Delete_AfterConfirmation_SoftDeletesFocusedTask()
    {
        var reducer = NewReducer();
        var state = Ready(reducer, Task("a", Monday, "A"));

        var result = Feed(reducer, state, Then(Key('d'), Key('y')));

        Assert.True(result.State.Cache.Single(task => task.Uid == "a").Deleted);
    }

    [Fact]
    public void ToggleRoutine_MarksFocusedDateAsShown()
    {
        var reducer = NewReducer();
        var state = Ready(reducer, Task("a", Monday, "A"));

        var result = Feed(reducer, state, Then(Key('r')));

        Assert.Contains(Monday, result.State.RoutineShownDates);
    }

    [Fact]
    public void ToggleCompletedVisibility_FlipsShowCompleted()
    {
        var reducer = NewReducer();
        var state = Ready(reducer, Task("a", Monday, "A"));
        Assert.False(state.ShowCompleted);

        var result = Feed(reducer, state, Then(Key('c')));

        Assert.True(result.State.ShowCompleted);
    }

    [Fact]
    public void Add_InvalidTaskText_DeclinesWithStatusAndCreatesNothing()
    {
        var reducer = NewReducer();
        var state = Ready(reducer, Task("a", Monday, "Existing"));

        var result = Feed(reducer, state, Then(Key('a'), Typed("0913-0909 trip"), Enter()));

        Assert.False(string.IsNullOrEmpty(result.State.StatusMessage));
        Assert.Single(result.State.Cache);
        Assert.DoesNotContain(result.Effects, effect => effect.Kind == ApplicationEffectKind.EnqueueTaskChanges);
    }

    [Fact]
    public void ResizeRequired_HelpRoundTrip_StaysResizeRequiredWhileBelowMinimum()
    {
        var reducer = NewReducer();
        var small = ApplicationReducer.HandleResize(Ready(reducer, Task("a", Monday, "A")), 100, 20);
        Assert.Equal(TerminalUiMode.ResizeRequired, small.Input.Mode);

        // Open Help from resize-required (allowed), then close it while the terminal is still too small.
        var afterOpen = reducer.HandleKey(small, Key('?'));
        Assert.Equal(TerminalUiMode.Help, afterOpen.State.Input.Mode);

        var afterClose = reducer.HandleKey(afterOpen.State, Escape());

        // Closing Help must return to resize-required, not Normal, because the size timer only re-emits a
        // resize event on a dimension change - so Normal would otherwise expose hidden commands unseen.
        Assert.Equal(TerminalUiMode.ResizeRequired, afterClose.State.Input.Mode);

        // A hidden command such as delete stays inert while below minimum instead of mutating unseen state.
        var afterHidden = reducer.HandleKey(afterClose.State, Key('d'));
        Assert.Equal(TerminalUiMode.ResizeRequired, afterHidden.State.Input.Mode);
        Assert.False(afterHidden.State.Cache.Single(task => task.Uid == "a").Deleted);
    }

    [Fact]
    public void ReadyBelowMinimum_FirstNormalKey_GatesWithoutRunningHiddenCommand()
    {
        var reducer = NewReducer();

        // Reaching Ready while the terminal is already below the minimum lands in Normal mode without any
        // resize event (the size timer only re-emits on a dimension change), so the first key must gate to
        // resize-required rather than run a hidden Normal command such as toggling completed visibility.
        var small = Ready(reducer, Task("a", Monday, "A")) with { Width = 100, Height = 20 };
        Assert.Equal(TerminalUiMode.Normal, small.Input.Mode);
        Assert.False(small.ShowCompleted);

        var afterKey = reducer.HandleKey(small, Key('c'));

        Assert.Equal(TerminalUiMode.ResizeRequired, afterKey.State.Input.Mode);
        Assert.False(afterKey.State.ShowCompleted);
    }

    [Fact]
    public void ReadyBelowMinimum_EnterOnAHiddenEditor_GatesInsteadOfCommitting()
    {
        var reducer = NewReducer();

        // Open the add editor and type a title at a supported size, then shrink below the minimum. The
        // size-gated renderer now hides the editor, so its Enter must be gated to resize-required rather
        // than commit a task the user can no longer see.
        var opened = Feed(reducer, Ready(reducer), Then(Key('A'), Typed("hidden task")));
        Assert.Equal(TerminalUiMode.Editor, opened.State.Input.Mode);

        var small = opened.State with { Width = 100, Height = 20 };
        var afterEnter = reducer.HandleKey(small, Enter());

        Assert.Equal(TerminalUiMode.ResizeRequired, afterEnter.State.Input.Mode);
        Assert.DoesNotContain(
            afterEnter.Effects, effect => effect.Kind == ApplicationEffectKind.EnqueueTaskChanges);
        Assert.Empty(afterEnter.State.Cache);
    }

    [Fact]
    public void ResizeBelowMinimumDuringEdit_GrowingBack_RestoresTheSuspendedDraft()
    {
        var reducer = NewReducer();

        // Open the add editor and type a draft at a supported size.
        var opened = Feed(reducer, Ready(reducer), Then(Key('A'), Typed("my draft")));
        Assert.Equal(TerminalUiMode.Editor, opened.State.Input.Mode);

        // Shrink below the minimum (a resize event leaves the still-open editor as-is), then press an
        // ignored key: it must gate to resize-required so no hidden command runs, without discarding the
        // suspended editor.
        var small = ApplicationReducer.HandleResize(opened.State, 100, 20);
        var gated = reducer.HandleKey(small, Key('x'));
        Assert.Equal(TerminalUiMode.ResizeRequired, gated.State.Input.Mode);

        // Growing back must resume the editor with its draft intact instead of resetting to Normal.
        var grown = ApplicationReducer.HandleResize(gated.State, 200, 60);
        Assert.Equal(TerminalUiMode.Editor, grown.Input.Mode);
        Assert.Equal("my draft", grown.Input.Editor.Text);
    }

    [Fact]
    public void ResizeBelowMinimumDuringDelete_HelpRoundTripThenGrow_RestoresTheConfirmation()
    {
        var reducer = NewReducer();

        // Enter delete confirmation on a focused task at a supported size.
        var confirming = Feed(reducer, FocusOn(Ready(reducer, Task("a", Monday, "A")), "a"), Then(Key('d')));
        Assert.Equal(TerminalUiMode.DeleteConfirmation, confirming.State.Input.Mode);

        // Shrink, then open and close Help while below the minimum: the confirmation is suspended, not lost.
        var small = ApplicationReducer.HandleResize(confirming.State, 100, 20);
        var afterHelp = Feed(reducer, small, Then(Key('?'), Escape()));
        Assert.Equal(TerminalUiMode.ResizeRequired, afterHelp.State.Input.Mode);

        // Growing back resumes the delete confirmation rather than dropping into Normal.
        var grown = ApplicationReducer.HandleResize(afterHelp.State, 200, 60);
        Assert.Equal(TerminalUiMode.DeleteConfirmation, grown.Input.Mode);
    }

    [Fact]
    public void ResizeBelowMinimumDuringEdit_GrowBackWhileHelpOpen_ThenCloseHelp_RestoresTheDraft()
    {
        var reducer = NewReducer();

        // Open the add editor and type a draft at a supported size, then shrink below the minimum.
        var opened = Feed(reducer, Ready(reducer), Then(Key('A'), Typed("my draft")));
        var small = ApplicationReducer.HandleResize(opened.State, 100, 20);

        // Open Help while below the minimum: the still-open editor draft is snapshotted into SuspendedInput.
        var withHelp = reducer.HandleKey(small, Key('?'));
        Assert.Equal(TerminalUiMode.Help, withHelp.State.Input.Mode);

        // Grow back to a usable size while Help is still open. The resize cannot consume the snapshot here
        // (the mode is Help, not ResizeRequired), so it must stay pending until Help finally closes.
        var grown = ApplicationReducer.HandleResize(withHelp.State, 200, 60);
        Assert.Equal(TerminalUiMode.Help, grown.Input.Mode);

        // Closing Help must now resume the suspended editor with its draft intact instead of dropping into
        // Normal and discarding it.
        var afterClose = reducer.HandleKey(grown, Key('?'));
        Assert.Equal(TerminalUiMode.Editor, afterClose.State.Input.Mode);
        Assert.Equal("my draft", afterClose.State.Input.Editor.Text);
    }

    [Fact]
    public void ResizeBelowMinimumDuringDelete_GrowBackWhileHelpOpen_ThenCloseHelp_RestoresTheConfirmation()
    {
        var reducer = NewReducer();

        // Enter delete confirmation on a focused task, shrink below the minimum, then open Help.
        var confirming = Feed(reducer, FocusOn(Ready(reducer, Task("a", Monday, "A")), "a"), Then(Key('d')));
        Assert.Equal(TerminalUiMode.DeleteConfirmation, confirming.State.Input.Mode);
        var small = ApplicationReducer.HandleResize(confirming.State, 100, 20);
        var withHelp = reducer.HandleKey(small, Key('?'));
        Assert.Equal(TerminalUiMode.Help, withHelp.State.Input.Mode);

        // Grow back while Help is still open, then close Help: the confirmation must resume rather than be
        // discarded into Normal, and the task must stay undeleted because the confirmation never ran.
        var grown = ApplicationReducer.HandleResize(withHelp.State, 200, 60);
        var afterClose = reducer.HandleKey(grown, Escape());
        Assert.Equal(TerminalUiMode.DeleteConfirmation, afterClose.State.Input.Mode);
        Assert.False(afterClose.State.Cache.Single(task => task.Uid == "a").Deleted);
    }

    private static ApplicationReducer NewReducer()
    {
        var dates = new FixedDates();
        var parser = new TaskTextParser(dates);
        var formatter = new TaskTextFormatter(dates);
        var counter = 0;
        return new ApplicationReducer(
            new TerminalInputReducer(parser, formatter),
            new OverviewProjectionService(dates),
            parser,
            formatter,
            () => $"new-{++counter}");
    }

    private static ApplicationState Ready(ApplicationReducer reducer, params TerminalTask[] cache)
    {
        // Seed a supported terminal size (the real app reads real dimensions when it constructs its state)
        // so Normal-mode commands are reachable; tests that need a too-small terminal set it explicitly.
        return reducer.Recompute(
            new ApplicationState { Phase = ApplicationPhase.Ready, Cache = cache, Width = 200, Height = 60 });
    }

    private static ApplicationState FocusOn(ApplicationState state, string uid)
    {
        var cells = new List<OverviewDay> { state.Projection.NoDate };
        cells.AddRange(state.Projection.Overdue);
        cells.AddRange(state.Projection.Current);
        cells.AddRange(state.Projection.Future);

        foreach (var cell in cells)
        {
            foreach (var task in cell.Tasks)
            {
                if (task.Task.Uid == uid)
                {
                    return state with { Focus = new TaskFocus { Uid = uid, Address = task.Address } };
                }
            }
        }

        throw new InvalidOperationException($"Task '{uid}' is not visible.");
    }

    private static ApplicationTransition Feed(
        ApplicationReducer reducer, ApplicationState state, IEnumerable<ConsoleKeyInfo> keys)
    {
        var transition = ApplicationTransition.Of(state);
        foreach (var key in keys)
        {
            transition = reducer.HandleKey(transition.State, key);
        }

        return transition;
    }

    private static IEnumerable<ConsoleKeyInfo> Then(params object[] parts)
    {
        foreach (var part in parts)
        {
            switch (part)
            {
                case ConsoleKeyInfo key:
                    yield return key;
                    break;
                case IEnumerable<ConsoleKeyInfo> keys:
                    foreach (var key in keys)
                    {
                        yield return key;
                    }

                    break;
                default:
                    throw new ArgumentException($"Unsupported key part '{part}'.", nameof(parts));
            }
        }
    }

    private static IEnumerable<ConsoleKeyInfo> Typed(string text)
    {
        return text.Select(Key);
    }

    private static ConsoleKeyInfo Key(char character)
    {
        var key = char.IsLetter(character) ? (ConsoleKey)char.ToUpperInvariant(character) : 0;
        return new ConsoleKeyInfo(character, key, shift: char.IsUpper(character), alt: false, control: false);
    }

    private static ConsoleKeyInfo Enter()
    {
        return new ConsoleKeyInfo('\r', ConsoleKey.Enter, shift: false, alt: false, control: false);
    }

    private static ConsoleKeyInfo Escape()
    {
        return new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, shift: false, alt: false, control: false);
    }

    private static ConsoleKeyInfo Space()
    {
        return new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, shift: false, alt: false, control: false);
    }

    private static ConsoleKeyInfo ShiftArrow(ConsoleKey key)
    {
        return new ConsoleKeyInfo('\0', key, shift: true, alt: false, control: false);
    }

    private static TerminalTask Task(
        string uid,
        DateOnly? date,
        string title = "Task",
        int order = 1,
        TerminalTaskType type = TerminalTaskType.Simple)
    {
        return new TerminalTask { Uid = uid, Title = title, Date = date, Order = order, Type = type };
    }

    private sealed class FixedDates : ILocalDateProvider, ITaskTextDateProvider
    {
        public DateOnly Today => new(2024, 6, 10);
    }
}
