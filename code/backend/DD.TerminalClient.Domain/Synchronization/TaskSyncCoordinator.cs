using DD.TerminalClient.Domain.Models;

namespace DD.TerminalClient.Domain.Synchronization;

// The pure save state machine for local task edits. It never touches disk, network, timers or the UI:
// it mutates its in-memory maps and returns the effects the application loop must run, then reacts to
// the outcomes the loop reports back. This mirrors the frontend TaskSyncService save path.
//
// Invariants:
//   - Every accepted mutation is persisted to the outbox (PersistOutbox effect) before its save is
//     dispatched, so an unsaved edit survives a crash between enqueue and network save.
//   - At most one batch is in flight; further edits accumulate in the pending map and win by Uid.
//   - A successful REST response is authoritative immediately: versions are applied and completed
//     entries cleared without ever waiting for the origin-suppressed hub echo of our own save.
//   - A transport failure requeues the still-current in-flight tasks and schedules exactly one
//     five-second retry, leaving the durable outbox untouched.
public sealed class TaskSyncCoordinator
{
    public static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);

    // The size of the batch handed to the current SaveBatch effect, captured when it is dispatched so a
    // failure reports the original count even if a concurrent Reset has since cleared the in-flight map.
    private int _inFlightBatchSize;

    public TaskSyncState State { get; } = new();

    // Accept local mutations (create/edit/complete/delete/move/reorder). Pending re-edits win per Uid.
    // Persists the outbox before dispatching, then starts saving if idle.
    public IReadOnlyList<TaskSyncEffect> EnqueueLocalChanges(IReadOnlyList<TerminalTask> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        var effects = new List<TaskSyncEffect>();
        foreach (var task in tasks)
            State.PendingByUid[task.Uid] = task;

        effects.Add(BuildPersistOutbox());
        PumpSaving(effects);
        return effects;
    }

    // Replay the durable outbox after a restart. The content is already on disk, so it is re-queued and
    // sent without re-persisting; the edited tasks are already visible from the hydrated cache.
    public IReadOnlyList<TaskSyncEffect> RestoreOutbox(IReadOnlyList<TerminalTask> outbox)
    {
        ArgumentNullException.ThrowIfNull(outbox);

        var effects = new List<TaskSyncEffect>();
        if (outbox.Count == 0)
            return effects;

        foreach (var task in outbox)
            State.PendingByUid[task.Uid] = task;

        PumpSaving(effects);
        return effects;
    }

    // The in-flight batch was accepted by the backend. Apply the response immediately: propagate the
    // returned versions into any newer pending re-edit, report the saved versions and the dropped
    // conflicts, clear the in-flight batch, persist the reduced outbox, then continue or finish.
    public IReadOnlyList<TaskSyncEffect> OnSaveSucceeded(IReadOnlyList<TerminalTask> savedTasks)
    {
        ArgumentNullException.ThrowIfNull(savedTasks);

        var effects = new List<TaskSyncEffect>();

        foreach (var saved in savedTasks)
        {
            if (State.PendingByUid.TryGetValue(saved.Uid, out var pending))
                State.PendingByUid[saved.Uid] = pending with { Version = saved.Version };
        }

        var savedUids = savedTasks.Select(task => task.Uid).ToHashSet(StringComparer.Ordinal);
        var conflicted = State.InFlightByUid.Values
            .Where(task => !savedUids.Contains(task.Uid) && !State.PendingByUid.ContainsKey(task.Uid))
            .ToList();

        var savedVersions = savedTasks
            .Select(task => new TerminalTaskVersion(task.Uid, task.Version))
            .ToList();
        effects.Add(TaskSyncEffect.SaveFinished(0, savedVersions, conflicted));

        State.InFlightByUid.Clear();
        effects.Add(BuildPersistOutbox());
        ContinueOrFinish(effects);
        return effects;
    }

    // The in-flight save failed with a transport error - nothing was saved. Report the batch size,
    // requeue the still-current in-flight tasks (a newer pending re-edit supersedes them), then
    // schedule a single retry. Reads the live in-flight map so a concurrent Reset (session expiry)
    // severs the batch and leaves the preserved outbox alone. The outbox is not rewritten here: its
    // merged content is unchanged since enqueue, and skipping the write avoids clobbering a Reset.
    public IReadOnlyList<TaskSyncEffect> OnSaveFailed()
    {
        var effects = new List<TaskSyncEffect>
        {
            TaskSyncEffect.SaveFinished(_inFlightBatchSize, [], []),
        };

        foreach (var entry in State.InFlightByUid)
        {
            if (!State.PendingByUid.ContainsKey(entry.Key))
                State.PendingByUid[entry.Key] = entry.Value;
        }

        State.InFlightByUid.Clear();
        effects.Add(TaskSyncEffect.ScheduleRetry(RetryDelay));
        return effects;
    }

    // The scheduled retry elapsed: send the next batch if anything is queued, otherwise finish.
    public IReadOnlyList<TaskSyncEffect> OnRetryElapsed()
    {
        var effects = new List<TaskSyncEffect>();
        ContinueOrFinish(effects);
        return effects;
    }

    // Drop the in-memory pending state on session expiry or sign-out. The persisted outbox is left
    // intact so the same user replays it on re-login; a different user has it wiped by the login guard.
    // IsSaving is left as-is: any in-flight save still settles and flips it off via ContinueOrFinish.
    public void Reset()
    {
        State.PendingByUid.Clear();
        State.InFlightByUid.Clear();
    }

    // Drop a task the backend won on a version conflict from both queues (a newer server version has
    // arrived), so the superseded local edit is abandoned and never retried. Returns whether anything
    // was removed. Used by TaskReconciler; mirrors deleting the Uid from tasksToSave and tasksInFlight
    // in the frontend processTasksOnlineUpdate. Keeps the coordinator the sole mutator of the maps.
    public bool DropConflictedEdit(string uid)
    {
        ArgumentNullException.ThrowIfNull(uid);

        var removedPending = State.PendingByUid.Remove(uid);
        var removedInFlight = State.InFlightByUid.Remove(uid);
        return removedPending || removedInFlight;
    }

    // The durable outbox contents: the in-flight batch merged with the pending re-edits, pending
    // winning per Uid because it holds the newest content. Exposed so the reconciler can persist the
    // reduced outbox after a conflict drops a pending edit, mirroring the frontend persistOutbox().
    public IReadOnlyList<TerminalTask> BuildOutboxContents()
    {
        var merged = new Dictionary<string, TerminalTask>(State.InFlightByUid, StringComparer.Ordinal);
        foreach (var entry in State.PendingByUid)
            merged[entry.Key] = entry.Value;

        return [.. merged.Values];
    }

    private void PumpSaving(List<TaskSyncEffect> effects)
    {
        if (State.IsSaving || State.PendingByUid.Count == 0)
            return;

        State.IsSaving = true;
        effects.Add(TaskSyncEffect.ReportSyncStatus(true));
        BeginBatch(effects);
    }

    private void ContinueOrFinish(List<TaskSyncEffect> effects)
    {
        if (State.PendingByUid.Count > 0)
        {
            BeginBatch(effects);
        }
        else
        {
            State.IsSaving = false;
            effects.Add(TaskSyncEffect.ReportSyncStatus(false));
        }
    }

    private void BeginBatch(List<TaskSyncEffect> effects)
    {
        State.InFlightByUid.Clear();
        foreach (var entry in State.PendingByUid)
            State.InFlightByUid[entry.Key] = entry.Value;
        State.PendingByUid.Clear();

        _inFlightBatchSize = State.InFlightByUid.Count;
        effects.Add(TaskSyncEffect.SaveBatch([.. State.InFlightByUid.Values]));
    }

    private TaskSyncEffect BuildPersistOutbox()
    {
        return TaskSyncEffect.PersistOutbox(BuildOutboxContents());
    }
}
