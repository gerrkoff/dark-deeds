using DD.TerminalClient.Domain.Overview;

namespace DD.TerminalClient.Domain.Navigation;

// Pure spatial navigation over the Overview grid. Given the current focus and a direction it returns
// the next focus, reading task counts and stable addresses straight from the projection so the same
// value is meaningful for movement and hit-testing. It never mutates state and returns the unchanged
// focus whenever a move is impossible (a boundary with no visible task beyond it).
//
// Vertical (Up/Down) walks the focused cell's stacked task list first; at the top/bottom of that list
// it crosses to the previous/next visual row - a row being every cell that shares a Section and Row -
// preferring the cell in the same Column and, on ties, the lower Column, skipping empty cells and
// empty rows (including empty Current calendar cells and section gaps). Entering a cell from above
// lands on its first task and from below on its last, so focus flows continuously top to bottom.
//
// Horizontal (Left/Right) stays inside the current visual row and jumps to the nearest non-empty cell
// in that direction without wrapping, preserving the approximate task index clamped to the target
// cell. The single full-width No Date cell therefore ignores Left/Right.
public static class TaskNavigationService
{
    public static TaskFocus Move(OverviewProjection projection, TaskFocus current, NavigationDirection direction)
    {
        ArgumentNullException.ThrowIfNull(projection);
        ArgumentNullException.ThrowIfNull(current);

        var cells = AllCells(projection);
        var currentCell = FindCell(cells, current.Address);

        // A stale focus (its cell or task index no longer exists) cannot move; the loop reconciles
        // focus separately after every re-projection.
        if (currentCell is null || current.Address.TaskIndex >= currentCell.Tasks.Count)
            return current;

        return direction switch
        {
            NavigationDirection.Up => MoveVertical(cells, currentCell, current, up: true),
            NavigationDirection.Down => MoveVertical(cells, currentCell, current, up: false),
            NavigationDirection.Left => MoveHorizontal(cells, currentCell, current, right: false),
            NavigationDirection.Right => MoveHorizontal(cells, currentCell, current, right: true),
            _ => current,
        };
    }

    private static TaskFocus MoveVertical(List<OverviewDay> cells, OverviewDay currentCell, TaskFocus current, bool up)
    {
        var taskIndex = current.Address.TaskIndex;

        // Move within the focused cell's task list before leaving the cell.
        if (up && taskIndex > 0)
            return FocusAt(currentCell, taskIndex - 1);
        if (!up && taskIndex < currentCell.Tasks.Count - 1)
            return FocusAt(currentCell, taskIndex + 1);

        // At the list boundary: cross to the nearest task in the previous/next visual row.
        var rows = VisualRows(cells);
        var rowIndex = rows.FindIndex(row =>
            row[0].Section == currentCell.Section && row[0].Row == currentCell.Row);
        if (rowIndex < 0)
            return current;

        var step = up ? -1 : 1;
        for (var i = rowIndex + step; i >= 0 && i < rows.Count; i += step)
        {
            var target = NearestColumnCell(rows[i], currentCell.Column);
            if (target is null)
                continue;

            // Enter from the top going down (first task), from the bottom going up (last task).
            var landingIndex = up ? target.Tasks.Count - 1 : 0;
            return FocusAt(target, landingIndex);
        }

        // No visual row in that direction holds a visible task: keep the current focus.
        return current;
    }

    private static TaskFocus MoveHorizontal(List<OverviewDay> cells, OverviewDay currentCell, TaskFocus current, bool right)
    {
        var rowCells = cells
            .Where(cell => cell.Section == currentCell.Section && cell.Row == currentCell.Row);

        var target = right
            ? rowCells
                .Where(cell => cell.Column > currentCell.Column && !cell.IsEmpty)
                .OrderBy(cell => cell.Column)
                .FirstOrDefault()
            : rowCells
                .Where(cell => cell.Column < currentCell.Column && !cell.IsEmpty)
                .OrderByDescending(cell => cell.Column)
                .FirstOrDefault();

        // No non-empty dated cell in that direction: no wrapping, keep the current focus.
        if (target is null)
            return current;

        var landingIndex = Math.Min(current.Address.TaskIndex, target.Tasks.Count - 1);
        return FocusAt(target, landingIndex);
    }

    // Every grid cell in reading order: the full-width No Date cell, then Overdue, Current and Future.
    // The projection already orders each section by (Row, Column), so the concatenation is globally
    // ordered by (Section, Row, Column).
    private static List<OverviewDay> AllCells(OverviewProjection projection)
    {
        return [projection.NoDate, .. projection.Overdue, .. projection.Current, .. projection.Future];
    }

    // Groups the cells into visual rows keyed by (Section, Row), ordered top to bottom, each row's
    // cells ordered left to right. Empty Current cells stay in their row so the same-column preference
    // is measured against the real calendar layout.
    private static List<List<OverviewDay>> VisualRows(List<OverviewDay> cells)
    {
        return cells
            .GroupBy(cell => (cell.Section, cell.Row))
            .OrderBy(group => (int)group.Key.Section)
            .ThenBy(group => group.Key.Row)
            .Select(group => group.OrderBy(cell => cell.Column).ToList())
            .ToList();
    }

    // The non-empty cell in a row nearest the given column, preferring an exact column match and, on
    // equal distance, the lower column. Null when the row has no visible task.
    private static OverviewDay? NearestColumnCell(List<OverviewDay> row, int column)
    {
        return row
            .Where(cell => !cell.IsEmpty)
            .OrderBy(cell => Math.Abs(cell.Column - column))
            .ThenBy(cell => cell.Column)
            .FirstOrDefault();
    }

    private static OverviewDay? FindCell(List<OverviewDay> cells, VisualTaskAddress address)
    {
        return cells.FirstOrDefault(cell =>
            cell.Section == address.Section && cell.Row == address.Row && cell.Column == address.Column);
    }

    private static TaskFocus FocusAt(OverviewDay cell, int taskIndex)
    {
        var task = cell.Tasks[taskIndex];
        return new TaskFocus { Uid = task.Task.Uid, Address = task.Address };
    }
}
