using DD.TerminalClient.Domain.Overview;

namespace DD.TerminalClient.Domain.Navigation;

// Pure navigation over the Overview's rendered top-to-bottom stream. Up/Down moves to the immediately
// previous/next visible task, crossing day and section boundaries without calendar-grid heuristics.
// Left/Right moves to the previous/next non-empty day and lands on its first task. A move at either
// outer boundary keeps the current focus unchanged.
public static class TaskNavigationService
{
    public static TaskFocus Move(OverviewProjection projection, TaskFocus current, NavigationDirection direction)
    {
        ArgumentNullException.ThrowIfNull(projection);
        ArgumentNullException.ThrowIfNull(current);

        var days = NavigableDays(projection);
        var currentDayIndex = days.FindIndex(day => HasAddress(day, current.Address));

        // A stale focus (its day or task index no longer exists) cannot move; the loop reconciles
        // focus separately after every re-projection.
        if (currentDayIndex < 0 ||
            current.Address.TaskIndex < 0 ||
            current.Address.TaskIndex >= days[currentDayIndex].Tasks.Count)
        {
            return current;
        }

        return direction switch
        {
            NavigationDirection.Up => MoveTask(days, currentDayIndex, current.Address.TaskIndex, up: true, current),
            NavigationDirection.Down => MoveTask(days, currentDayIndex, current.Address.TaskIndex, up: false, current),
            NavigationDirection.Left => MoveDay(days, currentDayIndex, left: true, current),
            NavigationDirection.Right => MoveDay(days, currentDayIndex, left: false, current),
            _ => current,
        };
    }

    private static TaskFocus MoveTask(
        List<OverviewDay> days,
        int dayIndex,
        int taskIndex,
        bool up,
        TaskFocus current)
    {
        if (up && taskIndex > 0)
            return FocusAt(days[dayIndex], taskIndex - 1);
        if (!up && taskIndex < days[dayIndex].Tasks.Count - 1)
            return FocusAt(days[dayIndex], taskIndex + 1);

        var targetDayIndex = dayIndex + (up ? -1 : 1);
        if (targetDayIndex < 0 || targetDayIndex >= days.Count)
            return current;

        var targetDay = days[targetDayIndex];
        return FocusAt(targetDay, up ? targetDay.Tasks.Count - 1 : 0);
    }

    private static TaskFocus MoveDay(List<OverviewDay> days, int dayIndex, bool left, TaskFocus current)
    {
        var targetDayIndex = dayIndex + (left ? -1 : 1);
        if (targetDayIndex < 0 || targetDayIndex >= days.Count)
            return current;

        return FocusAt(days[targetDayIndex], 0);
    }

    private static List<OverviewDay> NavigableDays(OverviewProjection projection)
    {
        return new[] { projection.NoDate }
            .Concat(projection.Overdue)
            .Concat(projection.Current)
            .Concat(projection.Future)
            .Where(day => day.Tasks.Count > 0)
            .ToList();
    }

    private static bool HasAddress(OverviewDay day, VisualTaskAddress address)
    {
        return day.Section == address.Section && day.Row == address.Row && day.Column == address.Column;
    }

    private static TaskFocus FocusAt(OverviewDay cell, int taskIndex)
    {
        var task = cell.Tasks[taskIndex];
        return new TaskFocus { Uid = task.Task.Uid, Address = task.Address };
    }
}
