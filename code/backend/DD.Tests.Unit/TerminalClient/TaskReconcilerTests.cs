using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Synchronization;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Ports the online-update/reconcile cases of code/frontend/tests/services/TaskSyncService.test.ts
// (processTasksOnlineUpdate) and code/frontend/tests/redux/overview-slice.test.ts (reconcileTasks) to
// the pure TaskReconciler, driving it exactly as the application loop would: reconcile a hub Update or
// a full snapshot, then apply the returned ReconciliationResult to a test cache. Adds the required
// stale-deletion, soft-delete retention, buffered-duplicate-version and reconnect-ordering cases.
public sealed class TaskReconcilerTests
{
    [Fact]
    public void ProcessOnlineUpdate_NotPendingLocally_AppliedAsIs()
    {
        var coordinator = new TaskSyncCoordinator();
        var reconciler = new TaskReconciler(coordinator);

        var incoming = NewTask("1", version: 5, title: "from server");
        var result = reconciler.ProcessOnlineUpdate([incoming]);

        Assert.Empty(result.TasksConflicted);
        Assert.Equal(incoming, Assert.Single(result.TasksToApply));
        Assert.Null(result.KeepUids);
        Assert.Null(result.OutboxToPersist);
    }

    [Fact]
    public void ProcessOnlineUpdate_SameVersionAsPendingEdit_Suppressed()
    {
        var coordinator = WithPendingEdits(NewTask("1", version: 5, title: "local edit"));
        var reconciler = new TaskReconciler(coordinator);

        var result = reconciler.ProcessOnlineUpdate([NewTask("1", version: 5, title: "from server")]);

        Assert.Empty(result.TasksConflicted);
        Assert.Empty(result.TasksToApply);
        Assert.Null(result.OutboxToPersist);
        Assert.Contains("1", coordinator.State.GetPendingUids());
    }

    [Fact]
    public void ProcessOnlineUpdate_OlderVersionThanPendingEdit_Suppressed()
    {
        var coordinator = WithPendingEdits(NewTask("1", version: 5, title: "local edit"));
        var reconciler = new TaskReconciler(coordinator);

        var result = reconciler.ProcessOnlineUpdate([NewTask("1", version: 4, title: "stale server")]);

        Assert.Empty(result.TasksConflicted);
        Assert.Empty(result.TasksToApply);
        Assert.Contains("1", coordinator.State.GetPendingUids());
    }

    [Fact]
    public void ProcessOnlineUpdate_NewerVersion_AppliedAndReportedConflict()
    {
        var coordinator = WithPendingEdits(NewTask("1", version: 5, title: "local edit"));
        var reconciler = new TaskReconciler(coordinator);

        var incoming = NewTask("1", version: 6, title: "from server");
        var result = reconciler.ProcessOnlineUpdate([incoming]);

        Assert.Equal(incoming, Assert.Single(result.TasksConflicted));
        Assert.Equal(incoming, Assert.Single(result.TasksToApply));
        Assert.DoesNotContain("1", coordinator.State.GetPendingUids());
    }

    [Fact]
    public void ProcessOnlineUpdate_NewerVersion_DropsFromBothPendingAndInFlight()
    {
        var coordinator = new TaskSyncCoordinator();

        // First enqueue moves the task in flight; the second re-edit queues it as pending too.
        coordinator.EnqueueLocalChanges([NewTask("1", version: 5)]);
        coordinator.EnqueueLocalChanges([NewTask("1", version: 5)]);
        Assert.True(coordinator.State.InFlight.ContainsKey("1"));
        Assert.True(coordinator.State.Pending.ContainsKey("1"));
        var reconciler = new TaskReconciler(coordinator);

        var result = reconciler.ProcessOnlineUpdate([NewTask("1", version: 6)]);

        Assert.Equal(new[] { "1" }, Uids(result.TasksConflicted));
        Assert.False(coordinator.State.Pending.ContainsKey("1"));
        Assert.False(coordinator.State.InFlight.ContainsKey("1"));
    }

    [Fact]
    public void ProcessOnlineUpdate_Conflict_PersistsReducedOutboxKeepingOtherPending()
    {
        var coordinator = WithPendingEdits(NewTask("a", version: 5), NewTask("b", version: 5));
        var reconciler = new TaskReconciler(coordinator);

        var result = reconciler.ProcessOnlineUpdate([NewTask("a", version: 6)]);

        Assert.Equal(new[] { "a" }, Uids(result.TasksConflicted));
        Assert.NotNull(result.OutboxToPersist);
        Assert.Equal(new[] { "b" }, Uids(result.OutboxToPersist!));
    }

    [Fact]
    public void ProcessOnlineUpdate_ConflictDropsLastPending_PersistsEmptyOutbox()
    {
        var coordinator = WithPendingEdits(NewTask("1", version: 5));
        var reconciler = new TaskReconciler(coordinator);

        var result = reconciler.ProcessOnlineUpdate([NewTask("1", version: 6)]);

        Assert.NotNull(result.OutboxToPersist);
        Assert.Empty(result.OutboxToPersist!);
    }

    [Fact]
    public void ProcessOnlineUpdate_NoConflict_DoesNotPersistOutbox()
    {
        var coordinator = new TaskSyncCoordinator();
        var reconciler = new TaskReconciler(coordinator);

        var result = reconciler.ProcessOnlineUpdate([NewTask("1", version: 1)]);

        Assert.Empty(result.TasksConflicted);
        Assert.Null(result.OutboxToPersist);
    }

    [Fact]
    public void ReconcileSnapshot_NewTask_AppliedAndKept()
    {
        var coordinator = new TaskSyncCoordinator();
        var reconciler = new TaskReconciler(coordinator);
        var cache = new List<TerminalTask>();

        var result = reconciler.ReconcileSnapshot([NewTask("new", version: 1)]);
        cache = ApplyToCache(cache, result);

        Assert.Equal(new[] { "new" }, Uids(cache));
        Assert.Contains("new", result.KeepUids!);
    }

    [Fact]
    public void ReconcileSnapshot_StaleTaskAbsentFromSnapshotAndPending_Removed()
    {
        var coordinator = new TaskSyncCoordinator();
        var reconciler = new TaskReconciler(coordinator);
        var cache = new List<TerminalTask> { NewTask("a"), NewTask("stale") };

        var result = reconciler.ReconcileSnapshot([NewTask("a", title: "updated")]);
        cache = ApplyToCache(cache, result);

        Assert.Equal(new[] { "a" }, Uids(cache));
        Assert.Equal("updated", cache[0].Title);
        Assert.DoesNotContain("stale", result.KeepUids!);
    }

    [Fact]
    public void ReconcileSnapshot_PendingTaskAbsentFromSnapshot_RetainedUnchanged()
    {
        var coordinator = WithPendingEdits(NewTask("pending", version: 2, title: "local edit"));
        var reconciler = new TaskReconciler(coordinator);
        var cache = new List<TerminalTask> { NewTask("pending", version: 2, title: "local edit") };

        var result = reconciler.ReconcileSnapshot([]);
        cache = ApplyToCache(cache, result);

        Assert.Equal(new[] { "pending" }, Uids(cache));
        Assert.Equal("local edit", cache[0].Title);
        Assert.Contains("pending", result.KeepUids!);
    }

    [Fact]
    public void ReconcileSnapshot_SoftDeleteSuppliedBySnapshot_Retained()
    {
        var coordinator = new TaskSyncCoordinator();
        var reconciler = new TaskReconciler(coordinator);
        var cache = new List<TerminalTask> { NewTask("d", deleted: true) };

        var result = reconciler.ReconcileSnapshot([NewTask("d", version: 1, deleted: true)]);
        cache = ApplyToCache(cache, result);

        var task = Assert.Single(cache);
        Assert.Equal("d", task.Uid);
        Assert.True(task.Deleted);
    }

    [Fact]
    public void ReconcileSnapshot_SoftDeleteAbsentFromSnapshotAndPending_Removed()
    {
        var coordinator = new TaskSyncCoordinator();
        var reconciler = new TaskReconciler(coordinator);
        var cache = new List<TerminalTask> { NewTask("live"), NewTask("d", deleted: true) };

        var result = reconciler.ReconcileSnapshot([NewTask("live")]);
        cache = ApplyToCache(cache, result);

        Assert.Equal(new[] { "live" }, Uids(cache));
        Assert.DoesNotContain("d", result.KeepUids!);
    }

    [Fact]
    public void ReconcileSnapshot_AppliesConflictRuleAgainstPendingEdit()
    {
        var coordinator = WithPendingEdits(NewTask("1", version: 5, title: "local edit"));
        var reconciler = new TaskReconciler(coordinator);
        var cache = new List<TerminalTask> { NewTask("1", version: 5, title: "local edit") };

        var incoming = NewTask("1", version: 6, title: "from server");
        var result = reconciler.ReconcileSnapshot([incoming]);
        cache = ApplyToCache(cache, result);

        Assert.Equal(incoming, Assert.Single(result.TasksConflicted));
        Assert.Equal("from server", Assert.Single(cache).Title);
        Assert.DoesNotContain("1", coordinator.State.GetPendingUids());
    }

    [Fact]
    public void Reconnect_ReplayBufferedUpdates_PreservesArrivalOrderAndLastWins()
    {
        var coordinator = new TaskSyncCoordinator();
        var reconciler = new TaskReconciler(coordinator);
        var cache = new List<TerminalTask> { NewTask("A", version: 1, title: "v1") };

        var snapshot = reconciler.ReconcileSnapshot([NewTask("A", version: 2, title: "v2")]);
        cache = ApplyToCache(cache, snapshot);

        // Server pushes that arrived while the reconnect snapshot was loading, drained from the hub
        // buffer in arrival order and replayed after reconciliation.
        TerminalTask[][] bufferedUpdates =
        [
            [NewTask("A", version: 3, title: "v3")],
            [NewTask("A", version: 4, title: "v4")],
        ];

        var appliedOrder = new List<string>();
        foreach (var update in bufferedUpdates)
        {
            var replay = reconciler.ProcessOnlineUpdate(update);
            appliedOrder.AddRange(replay.TasksToApply.Select(task => task.Title));
            cache = ApplyToCache(cache, replay);
        }

        Assert.Equal(new[] { "v3", "v4" }, appliedOrder);
        Assert.Equal(4, Assert.Single(cache).Version);
    }

    [Fact]
    public void Reconcile_BufferedDuplicateVersion_AppliedIdempotentlyWithoutConflict()
    {
        var coordinator = new TaskSyncCoordinator();
        var reconciler = new TaskReconciler(coordinator);
        var cache = new List<TerminalTask>();

        var snapshot = reconciler.ReconcileSnapshot([NewTask("A", version: 5, title: "v5")]);
        cache = ApplyToCache(cache, snapshot);

        // A buffered hub Update carrying the same task at the same version the snapshot already
        // applied - with no pending edit it re-applies harmlessly and is never a conflict.
        var duplicate = reconciler.ProcessOnlineUpdate([NewTask("A", version: 5, title: "v5")]);
        cache = ApplyToCache(cache, duplicate);

        Assert.Empty(duplicate.TasksConflicted);
        Assert.Equal(new[] { "A" }, Uids(duplicate.TasksToApply));
        Assert.Equal(5, Assert.Single(cache).Version);
    }

    // Establishes each task as an unsettled local edit by pushing it into the in-flight batch (a save
    // is "in progress" but never completed in the test). GetPendingEdit treats in-flight and pending
    // identically, exactly like the frontend `tasksToSave.get(uid) ?? tasksInFlight.get(uid)`.
    private static TaskSyncCoordinator WithPendingEdits(params TerminalTask[] tasks)
    {
        var coordinator = new TaskSyncCoordinator();
        coordinator.EnqueueLocalChanges(tasks);
        return coordinator;
    }

    // Mirrors the frontend cache reducers: reconcileTasks filters the cache by KeepUids then upserts
    // the snapshot; syncTasks (KeepUids null) upserts without removing anything.
    private static List<TerminalTask> ApplyToCache(IEnumerable<TerminalTask> cache, ReconciliationResult result)
    {
        var list = cache.ToList();

        if (result.KeepUids is not null)
        {
            var keep = result.KeepUids.ToHashSet(StringComparer.Ordinal);
            list = list.Where(task => keep.Contains(task.Uid)).ToList();
        }

        foreach (var task in result.TasksToApply)
        {
            var index = list.FindIndex(existing => existing.Uid == task.Uid);
            if (index >= 0)
                list[index] = task;
            else
                list.Add(task);
        }

        return list;
    }

    private static TerminalTask NewTask(
        string uid,
        int version = 0,
        string title = "Task",
        bool deleted = false)
    {
        return new TerminalTask
        {
            Uid = uid,
            Title = title,
            Date = null,
            Time = null,
            Order = 1,
            Completed = false,
            Deleted = deleted,
            Type = TerminalTaskType.Simple,
            IsProbable = false,
            Version = version,
        };
    }

    private static string[] Uids(IEnumerable<TerminalTask> tasks)
    {
        return tasks.Select(task => task.Uid).ToArray();
    }
}
