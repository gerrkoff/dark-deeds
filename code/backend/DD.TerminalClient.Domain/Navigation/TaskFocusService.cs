using DD.TerminalClient.Domain.Overview;

namespace DD.TerminalClient.Domain.Navigation;

// Pure focus resolution over the Overview grid, run after every re-projection so keyboard focus stays
// on a real visible task. It is deliberate about which task keeps focus:
//
//   - ResolveInitial picks the first visible task in reading order (No Date, then Overdue, Current and
//     Future; each section top to bottom, left to right), or null when nothing is visible.
//   - Reconcile preserves the focused task by Uid, so an edit, an explicit date move, a Routine
//     expand or a server-wins conflict that keeps the same Uid simply follows the task to its new
//     address. When the focused Uid is gone (deleted, completed-filtered, Routine-collapsed, removed
//     by a snapshot), it falls back to the nearest task at the old address: a surviving task in the
//     same cell clamped to the new length, otherwise the first task at or after the old address in
//     reading order, otherwise the last remaining task. It establishes initial focus when there was
//     none and content now exists, and returns null only when nothing is visible.
public static class TaskFocusService
{
    public static TaskFocus? ResolveInitial(OverviewProjection projection)
    {
        ArgumentNullException.ThrowIfNull(projection);

        var first = OrderedTasks(projection).FirstOrDefault();
        return first is null ? null : ToFocus(first);
    }

    public static TaskFocus? Reconcile(OverviewProjection projection, TaskFocus? previous)
    {
        ArgumentNullException.ThrowIfNull(projection);

        var ordered = OrderedTasks(projection);
        if (ordered.Count == 0)
            return null;

        // No prior focus (startup or everything was hidden) but content exists: focus the first task.
        if (previous is null)
            return ToFocus(ordered[0]);

        // Uid preservation: follow the same task wherever it now lives.
        var preserved = ordered.FirstOrDefault(task =>
            string.Equals(task.Task.Uid, previous.Uid, StringComparison.Ordinal));
        if (preserved is not null)
            return ToFocus(preserved);

        // The focused task disappeared: fall back to the nearest task at the old address.
        return ToFocus(NearestTo(ordered, previous.Address));
    }

    private static OverviewTask NearestTo(List<OverviewTask> ordered, VisualTaskAddress old)
    {
        // Prefer a surviving task in the exact old cell, clamped to its new task count. The cell's
        // tasks stay in ascending TaskIndex order, so index i is the task now at TaskIndex i.
        var sameCell = ordered
            .Where(task => task.Address.Section == old.Section
                && task.Address.Row == old.Row
                && task.Address.Column == old.Column)
            .ToList();
        if (sameCell.Count > 0)
            return sameCell[Math.Min(old.TaskIndex, sameCell.Count - 1)];

        // The old cell is gone: take the first task at or after the old address in reading order,
        // else the last remaining task.
        var after = ordered.FirstOrDefault(task => CompareAddress(task.Address, old) >= 0);
        return after ?? ordered[^1];
    }

    // Every visible task in reading order. AllCells is globally ordered by (Section, Row, Column) and
    // each cell already lists its tasks by TaskIndex, so the flattened sequence is the reading order.
    private static List<OverviewTask> OrderedTasks(OverviewProjection projection)
    {
        return AllCells(projection).SelectMany(cell => cell.Tasks).ToList();
    }

    private static List<OverviewDay> AllCells(OverviewProjection projection)
    {
        return [projection.NoDate, .. projection.Overdue, .. projection.Current, .. projection.Future];
    }

    private static int CompareAddress(VisualTaskAddress a, VisualTaskAddress b)
    {
        var bySection = ((int)a.Section).CompareTo((int)b.Section);
        if (bySection != 0)
            return bySection;

        var byRow = a.Row.CompareTo(b.Row);
        if (byRow != 0)
            return byRow;

        var byColumn = a.Column.CompareTo(b.Column);
        if (byColumn != 0)
            return byColumn;

        return a.TaskIndex.CompareTo(b.TaskIndex);
    }

    private static TaskFocus ToFocus(OverviewTask task)
    {
        return new TaskFocus { Uid = task.Task.Uid, Address = task.Address };
    }
}
