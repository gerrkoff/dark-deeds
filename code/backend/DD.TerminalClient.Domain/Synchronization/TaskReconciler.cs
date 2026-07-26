using DD.TerminalClient.Domain.Models;

namespace DD.TerminalClient.Domain.Synchronization;

// The pure conflict/reconciliation state machine for tasks pushed by the backend, whether a single
// real-time hub Update or a full snapshot reload. It mirrors the frontend TaskSyncService
// .processTasksOnlineUpdate plus the reloadTasks/reconcileTasks cache merge, applying the same
// backend-wins optimistic-concurrency rule as the React client:
//
//   - An incoming task with no local pending edit is applied as-is.
//   - While a local edit for the same Uid is pending (queued or in flight), an incoming task at the
//     same or older Version is suppressed so the unsaved change is not reverted.
//   - When the incoming Version is newer, the backend wins: the pending/in-flight copy is dropped,
//     the reduced outbox is persisted, the server task is applied, and a conflict is reported so the
//     UI can notify by task identity/title.
//
// It never touches disk, network or the UI: it reads and drops entries in the shared TaskSyncState
// through the coordinator (the sole map mutator) and returns a ReconciliationResult the application
// loop applies. Buffered hub updates that arrived during a snapshot load are replayed by feeding each,
// in arrival order, back through ProcessOnlineUpdate after ReconcileSnapshot; a failed snapshot never
// reaches the reconciler, so the hub client keeps its buffer and the cache is left untouched for the
// retry.
public sealed class TaskReconciler
{
    private readonly TaskSyncCoordinator _coordinator;

    public TaskReconciler(TaskSyncCoordinator coordinator)
    {
        ArgumentNullException.ThrowIfNull(coordinator);
        _coordinator = coordinator;
    }

    // Process an incremental set of tasks pushed by the hub (or replayed from the buffer in arrival
    // order). Applies the backend-wins rule per task and returns the tasks to upsert into the cache,
    // the conflicts to notify, and - when a conflict dropped a pending edit - the reduced outbox to
    // persist. Never removes cached tasks (KeepUids is null): a single push is not authoritative about
    // which tasks still exist.
    public ReconciliationResult ProcessOnlineUpdate(IReadOnlyList<TerminalTask> incoming)
    {
        ArgumentNullException.ThrowIfNull(incoming);

        var (tasksToApply, tasksConflicted, outboxToPersist) = ApplyConflictRule(incoming);

        return new ReconciliationResult
        {
            TasksToApply = tasksToApply,
            TasksConflicted = tasksConflicted,
            OutboxToPersist = outboxToPersist,
        };
    }

    // Reconcile a full snapshot from an initial load or a reconnect reload. Applies the same
    // backend-wins rule, then computes the Uids to keep - every snapshot Uid plus every still-pending
    // local edit - so the loop removes cached tasks that are absent from both. This is what deletes a
    // record removed on another client while offline, and it retains a soft-deleted task only while the
    // snapshot still supplies it or a local delete is still pending.
    public ReconciliationResult ReconcileSnapshot(IReadOnlyList<TerminalTask> snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var (tasksToApply, tasksConflicted, outboxToPersist) = ApplyConflictRule(snapshot);

        var keepUids = snapshot
            .Select(task => task.Uid)
            .Concat(_coordinator.State.GetPendingUids())
            .Distinct(StringComparer.Ordinal)
            .ToList();

        return new ReconciliationResult
        {
            TasksToApply = tasksToApply,
            TasksConflicted = tasksConflicted,
            KeepUids = keepUids,
            OutboxToPersist = outboxToPersist,
        };
    }

    private (List<TerminalTask> ToApply, List<TerminalTask> Conflicted, IReadOnlyList<TerminalTask>? Outbox)
        ApplyConflictRule(IReadOnlyList<TerminalTask> incoming)
    {
        var tasksToApply = new List<TerminalTask>();
        var tasksConflicted = new List<TerminalTask>();

        foreach (var task in incoming)
        {
            var pending = GetPendingEdit(task.Uid);

            if (pending is null)
            {
                // Not pending locally - apply the incoming task as-is.
                tasksToApply.Add(task);
                continue;
            }

            if (task.Version > pending.Version)
            {
                // Backend has a newer version than our pending edit - backend wins: drop the local
                // edit from both queues, report the conflict and overwrite local state.
                _coordinator.DropConflictedEdit(task.Uid);
                tasksConflicted.Add(task);
                tasksToApply.Add(task);
                continue;
            }

            // The pending edit is as new as the incoming task (same or older Version) - keep the
            // unsaved local change and skip the incoming copy so it is not reverted.
        }

        // Conflicts dropped pending edits, so the durable outbox must be rewritten to match; skip the
        // write entirely when nothing was dropped, exactly like the frontend guard.
        var outboxToPersist = tasksConflicted.Count > 0 ? _coordinator.BuildOutboxContents() : null;

        return (tasksToApply, tasksConflicted, outboxToPersist);
    }

    private TerminalTask? GetPendingEdit(string uid)
    {
        // A queued re-edit (Pending) holds newer content than an in-flight copy of the same task, so
        // it is preferred, matching the frontend `tasksToSave.get(uid) ?? tasksInFlight.get(uid)`.
        if (_coordinator.State.Pending.TryGetValue(uid, out var pending))
            return pending;

        if (_coordinator.State.InFlight.TryGetValue(uid, out var inFlight))
            return inFlight;

        return null;
    }
}
