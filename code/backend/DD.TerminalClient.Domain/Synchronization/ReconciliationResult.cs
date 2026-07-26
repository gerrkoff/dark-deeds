using DD.TerminalClient.Domain.Models;

namespace DD.TerminalClient.Domain.Synchronization;

// The outcome of reconciling backend-pushed tasks (a real-time hub Update or a full snapshot reload)
// against the local pending edits and the task cache. The application loop applies it: upsert
// TasksToApply into the cache, remove cached Uids absent from KeepUids (snapshots only), persist
// OutboxToPersist when present, and raise a conflict notification for each TasksConflicted entry.
// Mirrors the frontend OnlineUpdateResult plus the reloadTasks keepUids computation.
public sealed record ReconciliationResult
{
    private static readonly IReadOnlyList<TerminalTask> NoTasks = [];

    // Tasks to upsert into the cache: incoming tasks with no conflicting pending edit, plus the server
    // copies that won a version conflict. Arrival order is preserved so replayed buffered updates keep
    // their sequence.
    public IReadOnlyList<TerminalTask> TasksToApply { get; init; } = NoTasks;

    // Tasks the backend won on an optimistic-concurrency conflict (a newer server Version than a
    // pending edit). Their local edit was dropped; the UI notifies by task identity/title.
    public IReadOnlyList<TerminalTask> TasksConflicted { get; init; } = NoTasks;

    // For a full snapshot reconcile, the Uids to retain in the cache: every snapshot Uid plus every
    // still-pending local edit. Cached Uids absent from this set are stale (e.g. deleted on another
    // client while offline) and removed, so a soft-deleted record is retained only while it is supplied
    // by the snapshot or still pending. Null for an incremental online update, which never removes.
    public IReadOnlyList<string>? KeepUids { get; init; }

    // The reduced durable outbox to persist when a conflict dropped a pending edit; null when no
    // pending edit changed, so the loop skips an unnecessary write (mirroring the frontend guard that
    // only persists the outbox when at least one conflict was dropped).
    public IReadOnlyList<TerminalTask>? OutboxToPersist { get; init; }
}
