using DD.TerminalClient.Domain.Overview;

namespace DD.TerminalClient.Domain.Navigation;

// The currently focused task, carried by the application loop between renders. Uid preserves the
// focus across re-projections (an edited or moved task keeps focus even when its VisualTaskAddress
// changes), while Address anchors spatial movement and the nearest-old-address fallback when the
// focused task disappears. The application holds no focus (null) only when nothing is visible.
public sealed record TaskFocus
{
    public required string Uid { get; init; }

    public required VisualTaskAddress Address { get; init; }
}
