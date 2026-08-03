namespace DD.TerminalClient.Domain.Overview;

// The stable spatial coordinates of one visible task in the Overview grid: the section it lives in,
// the visual Row and Column of its day cell, and its TaskIndex within that cell's ordered visible
// list. Navigation moves between these addresses and rendering emits line metadata keyed by them, so
// the same value identifies a task for focus, movement and hit-testing. Column is the weekday offset
// (Monday = 0) for Current cells and the wrapped position (0..6) for Overdue and Future rows.
public sealed record VisualTaskAddress
{
    public required OverviewSection Section { get; init; }

    public required int Row { get; init; }

    public required int Column { get; init; }

    public required int TaskIndex { get; init; }
}
