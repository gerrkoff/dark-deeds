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
        return reducer.Recompute(new ApplicationState { Phase = ApplicationPhase.Ready, Cache = cache });
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
