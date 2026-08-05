using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Overview;
using DD.TerminalClient.Domain.Time;
using Moq;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Ports every case of code/frontend/tests/services/OverviewService.test.ts to the pure
// OverviewProjectionService and adds the required timezone/local-Monday, Overdue-contract,
// Routine-collapse, completed-toggle and all-empty cases, plus direct coverage of the stable
// section/row/column/task-index addresses and the two-rows-of-seven / seven-column wrapping layout.
public sealed class OverviewProjectionTests
{
    // Nov 4 2024 is a Monday, matching the frontend test's mocked dateService.monday.
    private static readonly DateOnly Monday = new(2024, 11, 4);

    private static readonly IReadOnlySet<DateOnly> NoRoutineShown = new HashSet<DateOnly>();

    [Fact]
    public void Project_Empty_MatchesFrontendEmptyModel()
    {
        var service = CreateService();

        var result = service.Project([], showCompleted: false, NoRoutineShown);

        Assert.Empty(result.NoDate.Tasks);
        Assert.Empty(result.Overdue);
        for (var i = 0; i < 14; i++)
        {
            Assert.Equal(Monday.AddDays(i), result.Current[i].Date);
            Assert.Empty(result.Current[i].Tasks);
        }

        Assert.Empty(result.Future);
    }

    [Fact]
    public void Project_NoDateTask_GoesToNoDate()
    {
        var service = CreateService();

        var result = service.Project([Task("1", date: null)], showCompleted: false, NoRoutineShown);

        Assert.Equal("1", Assert.Single(result.NoDate.Tasks).Task.Uid);
        Assert.Empty(result.Overdue);
        Assert.All(result.Current, cell => Assert.Empty(cell.Tasks));
        Assert.Empty(result.Future);
    }

    [Fact]
    public void Project_FutureTask_GoesToFuture()
    {
        var service = CreateService();
        var futureDate = new DateOnly(2024, 11, 18);

        var result = service.Project([Task("1", date: futureDate)], showCompleted: false, NoRoutineShown);

        Assert.Empty(result.NoDate.Tasks);
        Assert.Empty(result.Overdue);
        Assert.All(result.Current, cell => Assert.Empty(cell.Tasks));
        var futureCell = Assert.Single(result.Future);
        Assert.Equal(futureDate, futureCell.Date);
        Assert.Equal("1", Assert.Single(futureCell.Tasks).Task.Uid);
    }

    [Fact]
    public void Project_ExpiredTask_GoesToOverdue()
    {
        var service = CreateService();
        var pastDate = new DateOnly(2024, 11, 3);

        var result = service.Project([Task("1", date: pastDate)], showCompleted: false, NoRoutineShown);

        Assert.Empty(result.NoDate.Tasks);
        var overdueCell = Assert.Single(result.Overdue);
        Assert.Equal(pastDate, overdueCell.Date);
        Assert.Equal("1", Assert.Single(overdueCell.Tasks).Task.Uid);
        Assert.All(result.Current, cell => Assert.Empty(cell.Tasks));
        Assert.Empty(result.Future);
    }

    [Fact]
    public void Project_CompletedTask_HiddenWhenNotShown()
    {
        var service = CreateService();

        var result = service.Project(
            [Task("1", date: new DateOnly(2024, 11, 3), completed: true)],
            showCompleted: false,
            NoRoutineShown);

        Assert.Empty(result.NoDate.Tasks);
        Assert.Empty(result.Overdue);
        Assert.All(result.Current, cell => Assert.Empty(cell.Tasks));
        Assert.Empty(result.Future);
    }

    [Fact]
    public void Project_CompletedTask_ShownWhenShown()
    {
        var service = CreateService();
        var pastDate = new DateOnly(2024, 11, 3);

        var result = service.Project(
            [Task("1", date: pastDate, completed: true)],
            showCompleted: true,
            NoRoutineShown);

        var overdueCell = Assert.Single(result.Overdue);
        Assert.Equal(pastDate, overdueCell.Date);
        Assert.Equal("1", Assert.Single(overdueCell.Tasks).Task.Uid);
    }

    [Fact]
    public void Project_DeletedTask_Excluded()
    {
        var service = CreateService();

        var result = service.Project(
            [Task("1", date: new DateOnly(2024, 11, 3), deleted: true)],
            showCompleted: false,
            NoRoutineShown);

        Assert.Empty(result.NoDate.Tasks);
        Assert.Empty(result.Overdue);
        Assert.All(result.Current, cell => Assert.Empty(cell.Tasks));
        Assert.Empty(result.Future);
    }

    [Theory]
    [InlineData(2024, 11, 4, 2024, 11, 4)]
    [InlineData(2024, 11, 6, 2024, 11, 4)]
    [InlineData(2024, 11, 10, 2024, 11, 4)]
    [InlineData(2024, 11, 11, 2024, 11, 11)]
    public void Project_DerivesLocalMonday_FromToday(int ty, int tm, int td, int my, int mm, int md)
    {
        var service = CreateService(new DateOnly(ty, tm, td));

        var result = service.Project([], showCompleted: false, NoRoutineShown);

        var expectedMonday = new DateOnly(my, mm, md);
        Assert.Equal(14, result.Current.Count);
        Assert.Equal(expectedMonday, result.Current[0].Date);
        Assert.Equal(expectedMonday.AddDays(13), result.Current[13].Date);
    }

    [Fact]
    public void Project_DerivesWindow_FromMidweekToday()
    {
        var service = CreateService(new DateOnly(2024, 11, 6));

        var result = service.Project(
            [Task("past", date: new DateOnly(2024, 11, 3)), Task("today", date: new DateOnly(2024, 11, 4))],
            showCompleted: false,
            NoRoutineShown);

        Assert.Equal("past", Assert.Single(result.Overdue).Tasks.Single().Task.Uid);
        Assert.Equal("today", result.Current[0].Tasks.Single().Task.Uid);
    }

    [Fact]
    public void Project_Overdue_BucketsDatedTasksBeforeMonday_MondayStaysCurrent()
    {
        var service = CreateService();

        var result = service.Project(
            [
                Task("sunday", date: Monday.AddDays(-1)),
                Task("older", date: Monday.AddDays(-3)),
                Task("monday", date: Monday),
            ],
            showCompleted: false,
            NoRoutineShown);

        Assert.Equal(
            new[] { Monday.AddDays(-3), Monday.AddDays(-1) },
            result.Overdue.Select(cell => cell.Date!.Value).ToArray());
        Assert.DoesNotContain(result.Overdue, cell => cell.Date == Monday);
        Assert.Equal("monday", result.Current[0].Tasks.Single().Task.Uid);
    }

    [Fact]
    public void Project_WrapsOverdue_AtSevenColumns()
    {
        var service = CreateService();
        var tasks = Enumerable.Range(1, 10)
            .Select(i => Task($"o{i}", date: Monday.AddDays(-i)))
            .ToArray();

        var result = service.Project(tasks, showCompleted: false, NoRoutineShown);

        Assert.Equal(10, result.Overdue.Count);
        Assert.Equal((0, 6), (result.Overdue[6].Row, result.Overdue[6].Column));
        Assert.Equal((1, 0), (result.Overdue[7].Row, result.Overdue[7].Column));
    }

    [Fact]
    public void Project_CollapsesRoutineOnDatedCell_WhenDateNotShown()
    {
        var service = CreateService();

        var result = service.Project(
            [
                Task("s1", date: Monday, order: 1),
                Task("r1", date: Monday, order: 2, type: TerminalTaskType.Routine),
                Task("s2", date: Monday, order: 3),
            ],
            showCompleted: false,
            NoRoutineShown);

        var cell = result.Current[0];
        Assert.Equal(new[] { "s1", "s2" }, cell.Tasks.Select(task => task.Task.Uid).ToArray());
        Assert.Equal(1, cell.CollapsedRoutineCount);
        Assert.True(cell.HasCollapsedRoutineTasks);

        // A collapsed Routine leaves no gap in the visible task indexes.
        Assert.Equal(new[] { 0, 1 }, cell.Tasks.Select(task => task.Address.TaskIndex).ToArray());
    }

    [Fact]
    public void Project_ShowsRoutineOnDatedCell_WhenDateShown()
    {
        var service = CreateService();
        var shown = new HashSet<DateOnly> { Monday };

        var result = service.Project(
            [
                Task("s1", date: Monday, order: 1),
                Task("r1", date: Monday, order: 2, type: TerminalTaskType.Routine),
            ],
            showCompleted: false,
            shown);

        var cell = result.Current[0];
        Assert.Equal(new[] { "s1", "r1" }, cell.Tasks.Select(task => task.Task.Uid).ToArray());
        Assert.Equal(0, cell.CollapsedRoutineCount);
        Assert.False(cell.HasCollapsedRoutineTasks);
    }

    [Fact]
    public void Project_NeverCollapsesRoutine_InNoDateSection()
    {
        var service = CreateService();

        var result = service.Project(
            [Task("r", date: null, type: TerminalTaskType.Routine)],
            showCompleted: false,
            NoRoutineShown);

        Assert.Equal("r", Assert.Single(result.NoDate.Tasks).Task.Uid);
        Assert.Equal(0, result.NoDate.CollapsedRoutineCount);
        Assert.False(result.NoDate.HasCollapsedRoutineTasks);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Project_CollapsedRoutineCount_IncludesOnlyIncompleteTasks(bool showCompleted)
    {
        var service = CreateService();

        var result = service.Project(
            [
                Task("r-open", date: Monday, order: 1, type: TerminalTaskType.Routine),
                Task("r-done", date: Monday, order: 2, type: TerminalTaskType.Routine, completed: true),
            ],
            showCompleted,
            NoRoutineShown);

        var cell = result.Current[0];
        Assert.Empty(cell.Tasks);
        Assert.True(cell.IsEmpty);
        Assert.Equal(1, cell.CollapsedRoutineCount);
        Assert.True(cell.HasCollapsedRoutineTasks);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public void Project_CompletedCollapsedRoutines_FollowCompletedVisibility(
        bool showCompleted,
        bool expectedSummary)
    {
        var service = CreateService();

        var result = service.Project(
            [
                Task("r-done-1", date: Monday, order: 1, type: TerminalTaskType.Routine, completed: true),
                Task("r-done-2", date: Monday, order: 2, type: TerminalTaskType.Routine, completed: true),
            ],
            showCompleted,
            NoRoutineShown);

        var cell = result.Current[0];
        Assert.Empty(cell.Tasks);
        Assert.True(cell.IsEmpty);
        Assert.Equal(0, cell.CollapsedRoutineCount);
        Assert.Equal(expectedSummary, cell.HasCollapsedRoutineTasks);
    }

    [Fact]
    public void Project_CompletedToggle_HidesOrShowsCompleted()
    {
        var service = CreateService();
        TerminalTask[] tasks =
        [
            Task("done", date: Monday, order: 1, completed: true),
            Task("open", date: Monday, order: 2),
        ];

        var hidden = service.Project(tasks, showCompleted: false, NoRoutineShown);
        Assert.Equal(new[] { "open" }, hidden.Current[0].Tasks.Select(task => task.Task.Uid).ToArray());

        var shown = service.Project(tasks, showCompleted: true, NoRoutineShown);
        Assert.Equal(new[] { "done", "open" }, shown.Current[0].Tasks.Select(task => task.Task.Uid).ToArray());
    }

    [Fact]
    public void Project_AllEmpty_ProducesEmptySectionsAndFourteenEmptyCurrentCells()
    {
        var service = CreateService();

        var result = service.Project([], showCompleted: false, NoRoutineShown);

        Assert.Empty(result.NoDate.Tasks);
        Assert.Equal(0, result.NoDate.CollapsedRoutineCount);
        Assert.False(result.NoDate.HasCollapsedRoutineTasks);
        Assert.Empty(result.Overdue);
        Assert.Empty(result.Future);
        Assert.Equal(14, result.Current.Count);
        Assert.All(result.Current, cell => Assert.True(cell.IsEmpty));
    }

    [Fact]
    public void Project_NoDate_IsSingleFullWidthCellSortedByOrder()
    {
        var service = CreateService();

        var result = service.Project(
            [Task("n2", date: null, order: 2), Task("n1", date: null, order: 1)],
            showCompleted: false,
            NoRoutineShown);

        Assert.Equal(OverviewSection.NoDate, result.NoDate.Section);
        Assert.Equal(0, result.NoDate.Row);
        Assert.Equal(0, result.NoDate.Column);
        Assert.Null(result.NoDate.Date);
        Assert.Equal(new[] { "n1", "n2" }, result.NoDate.Tasks.Select(task => task.Task.Uid).ToArray());
    }

    [Fact]
    public void Project_SortsTasksByOrder_WithinCell()
    {
        var service = CreateService();

        var result = service.Project(
            [
                Task("c", date: Monday, order: 3),
                Task("a", date: Monday, order: 1),
                Task("b", date: Monday, order: 2),
            ],
            showCompleted: false,
            NoRoutineShown);

        Assert.Equal(new[] { "a", "b", "c" }, result.Current[0].Tasks.Select(task => task.Task.Uid).ToArray());
    }

    [Fact]
    public void Project_CurrentCells_FormTwoRowsOfSevenByWeekday()
    {
        var service = CreateService();

        var result = service.Project([], showCompleted: false, NoRoutineShown);

        Assert.Equal(14, result.Current.Count);
        for (var i = 0; i < 14; i++)
        {
            var cell = result.Current[i];
            Assert.Equal(Monday.AddDays(i), cell.Date);
            Assert.Equal(i / 7, cell.Row);
            Assert.Equal(i % 7, cell.Column);
            Assert.True(cell.IsEmpty);
        }
    }

    [Fact]
    public void Project_PreservesEmptyCurrentCells_AroundPopulatedOnes()
    {
        var service = CreateService();

        var result = service.Project(
            [Task("wed", date: Monday.AddDays(2))],
            showCompleted: false,
            NoRoutineShown);

        Assert.Equal(14, result.Current.Count);
        Assert.True(result.Current[0].IsEmpty);
        Assert.False(result.Current[2].IsEmpty);
        Assert.Equal("wed", result.Current[2].Tasks.Single().Task.Uid);
        Assert.True(result.Current[3].IsEmpty);
    }

    [Fact]
    public void Project_WrapsFuture_AtSevenColumns()
    {
        var service = CreateService();
        var futureStart = Monday.AddDays(14);
        var tasks = Enumerable.Range(0, 8)
            .Select(i => Task($"f{i}", date: futureStart.AddDays(i)))
            .ToArray();

        var result = service.Project(tasks, showCompleted: false, NoRoutineShown);

        Assert.Equal(8, result.Future.Count);
        Assert.Equal((0, 6), (result.Future[6].Row, result.Future[6].Column));
        Assert.Equal((1, 0), (result.Future[7].Row, result.Future[7].Column));
    }

    [Fact]
    public void Project_AssignsStableTaskAddresses()
    {
        var service = CreateService();

        var result = service.Project(
            [Task("a", date: Monday, order: 1), Task("b", date: Monday, order: 2)],
            showCompleted: false,
            NoRoutineShown);

        var cell = result.Current[0];
        var first = cell.Tasks[0].Address;
        Assert.Equal(OverviewSection.Current, first.Section);
        Assert.Equal(cell.Row, first.Row);
        Assert.Equal(cell.Column, first.Column);
        Assert.Equal(0, first.TaskIndex);
        Assert.Equal(1, cell.Tasks[1].Address.TaskIndex);
    }

    private static OverviewProjectionService CreateService(DateOnly? today = null)
    {
        var provider = new Mock<ILocalDateProvider>();
        provider.SetupGet(x => x.Today).Returns(today ?? Monday);
        return new OverviewProjectionService(provider.Object);
    }

    private static TerminalTask Task(
        string uid,
        DateOnly? date = null,
        int order = 0,
        bool completed = false,
        bool deleted = false,
        TerminalTaskType type = TerminalTaskType.Simple,
        string title = "Task")
    {
        return new TerminalTask
        {
            Uid = uid,
            Title = title,
            Date = date,
            Time = null,
            Order = order,
            Completed = completed,
            Deleted = deleted,
            Type = type,
            IsProbable = false,
            Version = 0,
        };
    }
}
