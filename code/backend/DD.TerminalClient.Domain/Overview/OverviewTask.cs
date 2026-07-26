using DD.TerminalClient.Domain.Models;

namespace DD.TerminalClient.Domain.Overview;

// One visible task inside a day cell, paired with its stable grid address. Wraps the immutable
// TerminalTask rather than replacing it, so rendering keeps the full task while navigation uses Address.
public sealed record OverviewTask
{
    public required TerminalTask Task { get; init; }

    public required VisualTaskAddress Address { get; init; }
}
