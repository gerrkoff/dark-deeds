namespace DD.TerminalClient.Domain.Overview;

// One day cell in the grid. Date is null only for the single No Date cell. Tasks are already filtered
// (deleted removed, completed optionally removed), Routine-collapsed for a dated cell whose date is not
// in the shown set, and sorted by Order. CollapsedRoutineCount is how many otherwise-visible Routine
// tasks that collapse hid (always 0 for No Date, where Routine is never collapsed). A cell with no
// visible task is Empty: Current keeps its empty cells for layout while navigation skips them.
public sealed record OverviewDay
{
    public required OverviewSection Section { get; init; }

    public required int Row { get; init; }

    public required int Column { get; init; }

    public DateOnly? Date { get; init; }

    public IReadOnlyList<OverviewTask> Tasks { get; init; } = [];

    public int CollapsedRoutineCount { get; init; }

    public bool IsEmpty => Tasks.Count == 0;
}
