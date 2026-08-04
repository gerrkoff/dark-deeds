using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Time;

namespace DD.TerminalClient.Domain.Overview;

// Projects the flat task cache into the Overview grid the terminal renders and navigates, porting the
// React OverviewService.getModel bucketing plus the DayCardList per-date Routine-collapse rule. Pure
// and deterministic: the only clock input is the injected ILocalDateProvider, from which it derives the
// current local Monday - the start of the visible period - exactly like the web dateService.monday.
public sealed class OverviewProjectionService(ILocalDateProvider localDateProvider)
{
    private const int VisibleColumns = 7;
    private const int CurrentDayCount = 14;

    public DateOnly Today => localDateProvider.Today;

    // Builds the projection. showCompleted toggles completed-task visibility everywhere; routineShownDates
    // are the dated days whose Routine tasks stay expanded (every other dated day collapses them).
    public OverviewProjection Project(
        IReadOnlyList<TerminalTask> tasks,
        bool showCompleted,
        IReadOnlySet<DateOnly> routineShownDates)
    {
        var currentStart = CurrentLocalMonday(localDateProvider.Today);
        var futureStart = currentStart.AddDays(CurrentDayCount);

        var noDateTasks = new List<TerminalTask>();
        var overdueByDate = new SortedDictionary<DateOnly, List<TerminalTask>>();
        var futureByDate = new SortedDictionary<DateOnly, List<TerminalTask>>();
        var currentByDate = new Dictionary<DateOnly, List<TerminalTask>>();

        for (var i = 0; i < CurrentDayCount; i++)
            currentByDate[currentStart.AddDays(i)] = [];

        foreach (var task in tasks)
        {
            if (task.Deleted)
                continue;

            var preservesCollapsedRoutineSummary = task.Type == TerminalTaskType.Routine
                && task.Date is { } routineDate
                && !routineShownDates.Contains(routineDate);
            if (!showCompleted && task.Completed && !preservesCollapsedRoutineSummary)
                continue;

            if (task.Date is not { } date)
            {
                noDateTasks.Add(task);
                continue;
            }

            if (date < currentStart)
                Bucket(overdueByDate, date).Add(task);
            else if (date >= futureStart)
                Bucket(futureByDate, date).Add(task);
            else
                currentByDate[date].Add(task);
        }

        return new OverviewProjection
        {
            NoDate = BuildCell(
                OverviewSection.NoDate,
                row: 0,
                column: 0,
                date: null,
                noDateTasks,
                showCompleted,
                routineShownDates),
            Overdue = BuildWrappedCells(OverviewSection.Overdue, overdueByDate, showCompleted, routineShownDates),
            Current = BuildCurrentCells(currentStart, currentByDate, showCompleted, routineShownDates),
            Future = BuildWrappedCells(OverviewSection.Future, futureByDate, showCompleted, routineShownDates),
        };
    }

    // Lays out only the nonempty dated cells (in ascending date order) into a grid that wraps every
    // seven columns, so Overdue and Future never exceed the seven visible date columns.
    private static List<OverviewDay> BuildWrappedCells(
        OverviewSection section,
        SortedDictionary<DateOnly, List<TerminalTask>> tasksByDate,
        bool showCompleted,
        IReadOnlySet<DateOnly> routineShownDates)
    {
        var cells = new List<OverviewDay>(tasksByDate.Count);
        var index = 0;

        foreach (var (date, rawTasks) in tasksByDate)
        {
            var row = index / VisibleColumns;
            var column = index % VisibleColumns;
            cells.Add(BuildCell(section, row, column, date, rawTasks, showCompleted, routineShownDates));
            index++;
        }

        return cells;
    }

    // Lays out exactly 14 Monday-based cells as two rows of seven, keeping empty days so the calendar
    // grid stays aligned; empty cells report IsEmpty and are skipped by focus.
    private static List<OverviewDay> BuildCurrentCells(
        DateOnly currentStart,
        Dictionary<DateOnly, List<TerminalTask>> currentByDate,
        bool showCompleted,
        IReadOnlySet<DateOnly> routineShownDates)
    {
        var cells = new List<OverviewDay>(CurrentDayCount);

        for (var i = 0; i < CurrentDayCount; i++)
        {
            var date = currentStart.AddDays(i);
            var row = i / VisibleColumns;
            var column = i % VisibleColumns;
            cells.Add(BuildCell(
                OverviewSection.Current,
                row,
                column,
                date,
                currentByDate[date],
                showCompleted,
                routineShownDates));
        }

        return cells;
    }

    // Filters and orders one cell's tasks: sort by Order (stable), collapse Routine tasks on a dated cell
    // whose date is not shown, record whether any exist while counting only incomplete ones, and stamp each
    // surviving task with its address. TaskIndex counts only visible tasks, so a collapsed Routine leaves
    // no gap.
    private static OverviewDay BuildCell(
        OverviewSection section,
        int row,
        int column,
        DateOnly? date,
        List<TerminalTask> rawTasks,
        bool showCompleted,
        IReadOnlySet<DateOnly> routineShownDates)
    {
        var collapseRoutine = date is { } value && !routineShownDates.Contains(value);
        var visible = new List<OverviewTask>(rawTasks.Count);
        var collapsedRoutineCount = 0;
        var hasCollapsedRoutineTasks = false;

        foreach (var task in rawTasks.OrderBy(task => task.Order))
        {
            if (collapseRoutine && task.Type == TerminalTaskType.Routine)
            {
                hasCollapsedRoutineTasks = true;
                if (!task.Completed)
                {
                    collapsedRoutineCount++;
                }

                continue;
            }

            if (!showCompleted && task.Completed)
            {
                continue;
            }

            visible.Add(new OverviewTask
            {
                Task = task,
                Address = new VisualTaskAddress
                {
                    Section = section,
                    Row = row,
                    Column = column,
                    TaskIndex = visible.Count,
                },
            });
        }

        return new OverviewDay
        {
            Section = section,
            Row = row,
            Column = column,
            Date = date,
            Tasks = visible,
            CollapsedRoutineCount = collapsedRoutineCount,
            HasCollapsedRoutineTasks = hasCollapsedRoutineTasks,
        };
    }

    private static List<TerminalTask> Bucket(SortedDictionary<DateOnly, List<TerminalTask>> map, DateOnly date)
    {
        if (!map.TryGetValue(date, out var list))
        {
            list = [];
            map[date] = list;
        }

        return list;
    }

    // The Monday on or before the given day, where the week starts on Monday (DayOfWeek has Sunday = 0).
    private static DateOnly CurrentLocalMonday(DateOnly today)
    {
        var daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;
        return today.AddDays(-daysSinceMonday);
    }
}
