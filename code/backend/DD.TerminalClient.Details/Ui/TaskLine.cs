using DD.TerminalClient.Domain.Overview;

namespace DD.TerminalClient.Details.Ui;

// Deterministic line metadata for one rendered task: the task's stable VisualTaskAddress and the
// zero-based index of the single content line it occupies within RenderedOverview.Lines. Because every
// task renders as exactly one ellipsized line, this index is exact and lets the viewport scroll the
// focused task's line into view and hit-test a screen row back to a task address.
public sealed record TaskLine
{
    public required VisualTaskAddress Address { get; init; }

    public required int LineIndex { get; init; }
}
