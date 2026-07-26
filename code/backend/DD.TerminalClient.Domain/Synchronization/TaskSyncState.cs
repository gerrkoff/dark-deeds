using DD.TerminalClient.Domain.Models;

namespace DD.TerminalClient.Domain.Synchronization;

// The in-memory save state owned by TaskSyncCoordinator: the pending re-edits waiting for the next
// batch and the single batch currently in flight, keyed by Uid. A pending entry always represents the
// newest accepted content for that Uid and wins over an in-flight copy of the same task. The maps are
// mutated only by the coordinator (same assembly); everyone else sees read-only views.
public sealed class TaskSyncState
{
    // True while a batch is being saved, including the wait between a failed attempt and its retry.
    public bool IsSaving { get; internal set; }

    public IReadOnlyDictionary<string, TerminalTask> Pending => PendingByUid;

    public IReadOnlyDictionary<string, TerminalTask> InFlight => InFlightByUid;

    internal Dictionary<string, TerminalTask> PendingByUid { get; } = new(StringComparer.Ordinal);

    internal Dictionary<string, TerminalTask> InFlightByUid { get; } = new(StringComparer.Ordinal);

    // Every Uid with an unsettled local edit, across both queues, without duplicates. Reconciliation
    // uses it to tell an incoming server task apart from one the user is still editing locally.
    public IReadOnlyList<string> GetPendingUids()
    {
        return PendingByUid.Keys
            .Concat(InFlightByUid.Keys)
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }
}
