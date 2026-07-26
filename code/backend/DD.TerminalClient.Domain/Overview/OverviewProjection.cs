namespace DD.TerminalClient.Domain.Overview;

// The full projected Overview grid the terminal renders and navigates. NoDate is the single full-width
// cell; Overdue, Current and Future are flat, ordered cell lists whose per-cell Row/Column encode the
// visual grid. Overdue is empty when nothing is overdue; Current always holds 14 cells (two rows of
// seven, empty cells included); Future holds one cell per nonempty dated day, wrapped at seven columns.
public sealed record OverviewProjection
{
    public required OverviewDay NoDate { get; init; }

    public IReadOnlyList<OverviewDay> Overdue { get; init; } = [];

    public IReadOnlyList<OverviewDay> Current { get; init; } = [];

    public IReadOnlyList<OverviewDay> Future { get; init; } = [];
}
