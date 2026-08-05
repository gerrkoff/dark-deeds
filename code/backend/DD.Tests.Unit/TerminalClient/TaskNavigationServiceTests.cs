using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Navigation;
using DD.TerminalClient.Domain.Overview;
using DD.TerminalClient.Domain.Time;
using Moq;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Exercises the pure stream navigation (TaskNavigationService) and focus fallback (TaskFocusService)
// against real projections produced by OverviewProjectionService, so every VisualTaskAddress is the
// genuine projected address. Covers task-order and day-order movement, empty days, every section
// boundary, filtered selection and remote removal plus the initial-focus, Uid-preservation and
// no-visible-task contracts.
public sealed class TaskNavigationServiceTests
{
    // Nov 4 2024 is a Monday, matching the projection tests' mocked local Monday.
    private static readonly DateOnly Monday = new(2024, 11, 4);

    // --- Vertical movement within a cell -------------------------------------------------------
    [Fact]
    public void Move_Down_WithinCell_AdvancesTaskIndex()
    {
        var projection = Project([
            Task("a", Monday, order: 1),
            Task("b", Monday, order: 2),
            Task("c", Monday, order: 3),
        ]);

        var moved = Move(projection, "a", NavigationDirection.Down);

        Assert.Equal("b", moved.Uid);
        Assert.Equal(1, moved.Address.TaskIndex);
    }

    [Fact]
    public void Move_Up_WithinCell_StepsTaskIndexBack()
    {
        var projection = Project([
            Task("a", Monday, order: 1),
            Task("b", Monday, order: 2),
            Task("c", Monday, order: 3),
        ]);

        var moved = Move(projection, "c", NavigationDirection.Up);

        Assert.Equal("b", moved.Uid);
    }

    [Fact]
    public void Move_Up_FromTopOfTopmostTask_IsNoOp()
    {
        var projection = Project([Task("only", Monday)]);
        var focus = Focus(projection, "only");

        Assert.Equal(focus, TaskNavigationService.Move(projection, focus, NavigationDirection.Up));
    }

    [Fact]
    public void Move_Down_FromBottomOfLastTask_IsNoOp()
    {
        var projection = Project([Task("only", Monday.AddDays(14))]);
        var focus = Focus(projection, "only");

        Assert.Equal(focus, TaskNavigationService.Move(projection, focus, NavigationDirection.Down));
    }

    // --- Vertical crossing between days ---------------------------------------------------------
    [Fact]
    public void Move_Down_AtListBottom_EntersNextDay_FirstTask()
    {
        var projection = Project([
            Task("top", Monday),
            Task("below", Monday.AddDays(7)),
        ]);

        var moved = Move(projection, "top", NavigationDirection.Down);

        Assert.Equal("below", moved.Uid);
        Assert.Equal(0, moved.Address.TaskIndex);
    }

    [Fact]
    public void Move_Up_AtListTop_EntersPreviousDay_LastTask()
    {
        var projection = Project([
            Task("above1", Monday, order: 1),
            Task("above2", Monday, order: 2),
            Task("below", Monday.AddDays(7)),
        ]);

        var moved = Move(projection, "below", NavigationDirection.Up);

        Assert.Equal("above2", moved.Uid);
    }

    [Fact]
    public void Move_Down_NextDay_LandsOnFirstTaskRegardlessOfSourceIndex()
    {
        var projection = Project([
            Task("a1", Monday, order: 1),
            Task("a2", Monday, order: 2),
            Task("a3", Monday, order: 3),
            Task("b1", Monday.AddDays(7), order: 1),
            Task("b2", Monday.AddDays(7), order: 2),
        ]);

        var moved = Move(projection, "a3", NavigationDirection.Down);

        Assert.Equal("b1", moved.Uid);
        Assert.Equal(0, moved.Address.TaskIndex);
    }

    [Fact]
    public void Move_Down_FollowsDateOrderInsteadOfCalendarColumn()
    {
        // Wednesday is followed by next Monday in the rendered stream even though next Wednesday
        // occupies the same calendar column.
        var projection = Project([
            Task("src", Monday.AddDays(2)),
            Task("next", Monday.AddDays(7)),
            Task("same-column", Monday.AddDays(9)),
        ]);

        var moved = Move(projection, "src", NavigationDirection.Down);

        Assert.Equal("next", moved.Uid);
    }

    [Fact]
    public void Move_Down_CrossesCurrentWeekBoundaryInDateOrder()
    {
        var projection = Project([
            Task("r0", Monday.AddDays(3)),
            Task("r1", Monday.AddDays(10)),
        ]);

        var moved = Move(projection, "r0", NavigationDirection.Down);

        Assert.Equal("r1", moved.Uid);
        Assert.Equal(OverviewSection.Current, moved.Address.Section);
        Assert.Equal(1, moved.Address.Row);
    }

    [Fact]
    public void Move_Up_CrossesCurrentWeekBoundaryInDateOrder()
    {
        var projection = Project([
            Task("r0", Monday.AddDays(3)),
            Task("r1", Monday.AddDays(10)),
        ]);

        var moved = Move(projection, "r1", NavigationDirection.Up);

        Assert.Equal("r0", moved.Uid);
        Assert.Equal(0, moved.Address.Row);
    }

    [Fact]
    public void Move_Down_Future_EntersNextRenderedDate()
    {
        var futureStart = Monday.AddDays(14);
        var tasks = Enumerable.Range(0, 8)
            .Select(i => Task($"f{i}", futureStart.AddDays(i)))
            .ToArray();
        var projection = Project(tasks);

        var moved = Move(projection, "f0", NavigationDirection.Down);

        Assert.Equal("f1", moved.Uid);
        Assert.Equal((OverviewSection.Future, 0, 1), (moved.Address.Section, moved.Address.Row, moved.Address.Column));
    }

    // --- Vertical crossing between sections -----------------------------------------------------
    [Fact]
    public void Move_Down_FromNoDate_EntersOverdueWhenPresent()
    {
        var projection = Project([
            Task("nodate", date: null),
            Task("overdue", Monday.AddDays(-1)),
            Task("current", Monday),
        ]);

        var moved = Move(projection, "nodate", NavigationDirection.Down);

        Assert.Equal("overdue", moved.Uid);
        Assert.Equal(OverviewSection.Overdue, moved.Address.Section);
    }

    [Fact]
    public void Move_Down_FromNoDate_SkipsToCurrentWhenNoOverdue()
    {
        var projection = Project([
            Task("nodate", date: null),
            Task("current", Monday),
        ]);

        var moved = Move(projection, "nodate", NavigationDirection.Down);

        Assert.Equal("current", moved.Uid);
        Assert.Equal(OverviewSection.Current, moved.Address.Section);
    }

    [Fact]
    public void Move_Down_FromOverdue_EntersCurrent()
    {
        var projection = Project([
            Task("overdue", Monday.AddDays(-1)),
            Task("current", Monday),
        ]);

        var moved = Move(projection, "overdue", NavigationDirection.Down);

        Assert.Equal("current", moved.Uid);
        Assert.Equal((OverviewSection.Current, 0), (moved.Address.Section, moved.Address.Row));
    }

    [Fact]
    public void Move_Down_FromLastCurrentRow_EntersFuture()
    {
        var projection = Project([
            Task("current", Monday.AddDays(7)),
            Task("future", Monday.AddDays(14)),
        ]);

        var moved = Move(projection, "current", NavigationDirection.Down);

        Assert.Equal("future", moved.Uid);
        Assert.Equal(OverviewSection.Future, moved.Address.Section);
    }

    [Fact]
    public void Move_Up_FromCurrent_EntersOverdue()
    {
        var projection = Project([
            Task("overdue", Monday.AddDays(-1)),
            Task("current", Monday),
        ]);

        var moved = Move(projection, "current", NavigationDirection.Up);

        Assert.Equal("overdue", moved.Uid);
        Assert.Equal(OverviewSection.Overdue, moved.Address.Section);
    }

    [Fact]
    public void Move_Up_FromFuture_EntersCurrent()
    {
        var projection = Project([
            Task("current", Monday.AddDays(7)),
            Task("future", Monday.AddDays(14)),
        ]);

        var moved = Move(projection, "future", NavigationDirection.Up);

        Assert.Equal("current", moved.Uid);
        Assert.Equal((OverviewSection.Current, 1), (moved.Address.Section, moved.Address.Row));
    }

    // --- Day movement ---------------------------------------------------------------------------
    [Fact]
    public void Move_Right_ToNextRenderedDay_SkipsDaysWithoutTasks()
    {
        // Monday (column 0) and Wednesday (column 2) hold tasks; Tuesday (column 1) is empty.
        var projection = Project([
            Task("mon", Monday),
            Task("wed", Monday.AddDays(2)),
        ]);

        var moved = Move(projection, "mon", NavigationDirection.Right);

        Assert.Equal("wed", moved.Uid);
        Assert.Equal(2, moved.Address.Column);
    }

    [Fact]
    public void Move_Left_ToPreviousRenderedDay()
    {
        var projection = Project([
            Task("mon", Monday),
            Task("wed", Monday.AddDays(2)),
        ]);

        var moved = Move(projection, "wed", NavigationDirection.Left);

        Assert.Equal("mon", moved.Uid);
    }

    [Fact]
    public void Move_Right_LandsOnFirstTaskOfNextDay()
    {
        var projection = Project([
            Task("a1", Monday, order: 1),
            Task("a2", Monday, order: 2),
            Task("a3", Monday, order: 3),
            Task("b1", Monday.AddDays(1), order: 1),
            Task("b2", Monday.AddDays(1), order: 2),
            Task("b3", Monday.AddDays(1), order: 3),
        ]);

        var moved = Move(projection, "a2", NavigationDirection.Right);

        Assert.Equal("b1", moved.Uid);
        Assert.Equal(0, moved.Address.TaskIndex);
    }

    [Fact]
    public void Move_Right_FromLastTask_LandsOnFirstTaskOfShorterDay()
    {
        var projection = Project([
            Task("a1", Monday, order: 1),
            Task("a2", Monday, order: 2),
            Task("a3", Monday, order: 3),
            Task("b1", Monday.AddDays(1), order: 1),
        ]);

        var moved = Move(projection, "a3", NavigationDirection.Right);

        Assert.Equal("b1", moved.Uid);
        Assert.Equal(0, moved.Address.TaskIndex);
    }

    [Fact]
    public void Move_Right_WithNoLaterDay_DoesNotWrap()
    {
        var projection = Project([
            Task("mon", Monday),
            Task("tue", Monday.AddDays(1)),
        ]);
        var focus = Focus(projection, "tue");

        Assert.Equal(focus, TaskNavigationService.Move(projection, focus, NavigationDirection.Right));
    }

    [Fact]
    public void Move_LeftOrRight_WithOnlyNoDate_IsNoOp()
    {
        var projection = Project([
            Task("n1", date: null, order: 1),
            Task("n2", date: null, order: 2),
        ]);
        var focus = Focus(projection, "n2");

        Assert.Equal(focus, TaskNavigationService.Move(projection, focus, NavigationDirection.Left));
        Assert.Equal(focus, TaskNavigationService.Move(projection, focus, NavigationDirection.Right));
    }

    [Fact]
    public void Move_Right_FromNoDate_EntersFirstDatedDay()
    {
        var projection = Project([
            Task("nodate", date: null),
            Task("current", Monday),
        ]);

        Assert.Equal("current", Move(projection, "nodate", NavigationDirection.Right).Uid);
    }

    [Fact]
    public void Move_Left_FromCurrent_EntersNoDate()
    {
        var projection = Project([
            Task("nodate", date: null),
            Task("current", Monday),
        ]);

        Assert.Equal("nodate", Move(projection, "current", NavigationDirection.Left).Uid);
    }

    [Fact]
    public void Move_Right_CrossesFromCurrentToFuture()
    {
        var projection = Project([
            Task("current", Monday.AddDays(13)),
            Task("future", Monday.AddDays(14)),
        ]);

        Assert.Equal("future", Move(projection, "current", NavigationDirection.Right).Uid);
    }

    // --- Initial focus --------------------------------------------------------------------------
    [Fact]
    public void ResolveInitial_Empty_ReturnsNull()
    {
        Assert.Null(TaskFocusService.ResolveInitial(Project([])));
    }

    [Fact]
    public void ResolveInitial_PrefersNoDate_InReadingOrder()
    {
        var projection = Project([
            Task("current", Monday),
            Task("nodate", date: null),
        ]);

        Assert.Equal("nodate", TaskFocusService.ResolveInitial(projection)!.Uid);
    }

    [Fact]
    public void ResolveInitial_PrefersOverdueBeforeCurrent_WhenNoNoDate()
    {
        var projection = Project([
            Task("current", Monday),
            Task("overdue", Monday.AddDays(-1)),
        ]);

        Assert.Equal("overdue", TaskFocusService.ResolveInitial(projection)!.Uid);
    }

    [Fact]
    public void ResolveInitial_PreferredDate_PicksItsFirstTask()
    {
        var projection = Project([
            Task("nodate", date: null),
            Task("today-first", Monday, order: 1),
            Task("today-second", Monday, order: 2),
        ]);

        Assert.Equal("today-first", TaskFocusService.ResolveInitial(projection, Monday)!.Uid);
    }

    // --- Focus reconciliation -------------------------------------------------------------------
    [Fact]
    public void Reconcile_NoPreviousFocus_WithContent_EstablishesInitialFocus()
    {
        var projection = Project([Task("first", Monday)]);

        Assert.Equal("first", TaskFocusService.Reconcile(projection, previous: null)!.Uid);
    }

    [Fact]
    public void Reconcile_NoPreviousFocus_PrefersRequestedDate()
    {
        var projection = Project([
            Task("nodate", date: null),
            Task("today", Monday),
        ]);

        Assert.Equal("today", TaskFocusService.Reconcile(projection, previous: null, Monday)!.Uid);
    }

    [Fact]
    public void Reconcile_EmptyProjection_ReturnsNull()
    {
        var before = Project([Task("gone", Monday)]);
        var previous = Focus(before, "gone");

        Assert.Null(TaskFocusService.Reconcile(Project([]), previous));
    }

    [Fact]
    public void Reconcile_PreservesFocusByUid_AcrossExplicitMove()
    {
        var before = Project([Task("x", Monday)]);
        var previous = Focus(before, "x");

        // The same task now lives on Wednesday: focus follows the Uid to the new address.
        var after = Project([Task("x", Monday.AddDays(2))]);
        var reconciled = TaskFocusService.Reconcile(after, previous)!;

        Assert.Equal("x", reconciled.Uid);
        Assert.Equal(Focus(after, "x").Address, reconciled.Address);
        Assert.NotEqual(previous.Address, reconciled.Address);
    }

    [Fact]
    public void Reconcile_ServerWinsConflict_KeepsFocusByUidWhenTaskSurvives()
    {
        var before = Project([Task("x", Monday, version: 1)]);
        var previous = Focus(before, "x");

        var after = Project([Task("x", Monday, version: 2)]);

        Assert.Equal("x", TaskFocusService.Reconcile(after, previous)!.Uid);
    }

    [Fact]
    public void Reconcile_DeletedMiddleTask_FallsBackToTaskThatTookItsSlot()
    {
        var before = Project([
            Task("a", Monday, order: 1),
            Task("b", Monday, order: 2),
            Task("c", Monday, order: 3),
        ]);
        var previous = Focus(before, "b");

        var after = Project([
            Task("a", Monday, order: 1),
            Task("c", Monday, order: 3),
        ]);

        Assert.Equal("c", TaskFocusService.Reconcile(after, previous)!.Uid);
    }

    [Fact]
    public void Reconcile_DeletedLastTask_StaysInSameCellOnNewLast()
    {
        var before = Project([
            Task("a", Monday, order: 1),
            Task("b", Monday, order: 2),
            Task("c", Monday, order: 3),
        ]);
        var previous = Focus(before, "c");

        var after = Project([
            Task("a", Monday, order: 1),
            Task("b", Monday, order: 2),
        ]);

        Assert.Equal("b", TaskFocusService.Reconcile(after, previous)!.Uid);
    }

    [Fact]
    public void Reconcile_CompletedFilterHidesFocusedTask_FallsBackWithinCell()
    {
        var tasks = new[]
        {
            Task("a", Monday, order: 1),
            Task("b", Monday, order: 2, completed: true),
            Task("c", Monday, order: 3),
        };
        var before = Project(tasks, showCompleted: true);
        var previous = Focus(before, "b");

        var after = Project(tasks, showCompleted: false);

        Assert.Equal("c", TaskFocusService.Reconcile(after, previous)!.Uid);
    }

    [Fact]
    public void Reconcile_RoutineCollapseHidesFocusedTask_FallsBackWithinCell()
    {
        var tasks = new[]
        {
            Task("s1", Monday, order: 1),
            Task("r1", Monday, order: 2, type: TerminalTaskType.Routine),
            Task("s2", Monday, order: 3),
        };
        var shown = new HashSet<DateOnly> { Monday };
        var before = Project(tasks, routineShown: shown);
        var previous = Focus(before, "r1");

        var after = Project(tasks, routineShown: new HashSet<DateOnly>());

        Assert.Equal("s2", TaskFocusService.Reconcile(after, previous)!.Uid);
    }

    [Fact]
    public void Reconcile_RemoteRemovalOfWholeCell_FallsBackToNearestOldAddress()
    {
        var before = Project([Task("gone", Monday)]);
        var previous = Focus(before, "gone");

        // A snapshot removed the focused task and its whole cell; the nearest task at or after the
        // old address in reading order is the Tuesday task.
        var after = Project([Task("neighbor", Monday.AddDays(1))]);

        Assert.Equal("neighbor", TaskFocusService.Reconcile(after, previous)!.Uid);
    }

    // --- Helpers --------------------------------------------------------------------------------
    private static TaskFocus Move(OverviewProjection projection, string uid, NavigationDirection direction)
    {
        return TaskNavigationService.Move(projection, Focus(projection, uid), direction);
    }

    private static TaskFocus Focus(OverviewProjection projection, string uid)
    {
        var task = AllTasks(projection).First(item => string.Equals(item.Task.Uid, uid, StringComparison.Ordinal));
        return new TaskFocus { Uid = task.Task.Uid, Address = task.Address };
    }

    private static IEnumerable<OverviewTask> AllTasks(OverviewProjection projection)
    {
        return new[] { projection.NoDate }
            .Concat(projection.Overdue)
            .Concat(projection.Current)
            .Concat(projection.Future)
            .SelectMany(cell => cell.Tasks);
    }

    private static OverviewProjection Project(
        IReadOnlyList<TerminalTask> tasks,
        bool showCompleted = false,
        IReadOnlySet<DateOnly>? routineShown = null)
    {
        var provider = new Mock<ILocalDateProvider>();
        provider.SetupGet(x => x.Today).Returns(Monday);
        var service = new OverviewProjectionService(provider.Object);
        return service.Project(tasks, showCompleted, routineShown ?? new HashSet<DateOnly>());
    }

    private static TerminalTask Task(
        string uid,
        DateOnly? date = null,
        int order = 0,
        bool completed = false,
        bool deleted = false,
        TerminalTaskType type = TerminalTaskType.Simple,
        int version = 0)
    {
        return new TerminalTask
        {
            Uid = uid,
            Title = uid,
            Date = date,
            Time = null,
            Order = order,
            Completed = completed,
            Deleted = deleted,
            Type = type,
            IsProbable = false,
            Version = version,
        };
    }
}
