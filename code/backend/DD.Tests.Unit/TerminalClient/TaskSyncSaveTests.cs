using DD.TerminalClient.Domain.Models;
using DD.TerminalClient.Domain.Synchronization;
using Xunit;

namespace DD.Tests.Unit.TerminalClient;

// Ports the save-path cases of code/frontend/tests/services/TaskSyncService.test.ts (enqueue, save,
// retry, in-flight re-edit, reset, outbox and version behaviour) to the pure TaskSyncCoordinator state
// machine, driving it exactly as the application loop would: call a method, run the returned effects.
// The online-update/reconcile cases live with the reconciler (Task 9). Adds an explicit no-own-hub-echo
// case: a save settles from the REST response alone, never waiting for the origin-suppressed hub echo.
public sealed class TaskSyncSaveTests
{
    [Fact]
    public void Save_BackendRejectsOnVersionConflict_NotRetriedAndReportedConflicted()
    {
        var coordinator = new TaskSyncCoordinator();

        var enqueued = coordinator.EnqueueLocalChanges([NewTask("1", version: 0)]);
        var completed = coordinator.OnSaveSucceeded([]);

        Assert.Equal(1, CountEffects(enqueued, TaskSyncEffectKind.SaveBatch));
        Assert.Equal(0, CountEffects(completed, TaskSyncEffectKind.SaveBatch));
        var finished = SingleEffect(completed, TaskSyncEffectKind.SaveFinished);
        Assert.Equal(new[] { "1" }, Uids(finished.Conflicted));
        Assert.Empty(coordinator.State.GetPendingUids());
    }

    [Fact]
    public void Save_Success_ReportsSavedVersionsAndNoFailures()
    {
        var coordinator = new TaskSyncCoordinator();

        coordinator.EnqueueLocalChanges([NewTask("1", version: 0)]);
        var completed = coordinator.OnSaveSucceeded([NewTask("1", version: 1)]);

        var finished = SingleEffect(completed, TaskSyncEffectKind.SaveFinished);
        Assert.Equal(0, finished.NotSaved);
        Assert.Empty(finished.Conflicted);
        var saved = Assert.Single(finished.Saved);
        Assert.Equal("1", saved.Uid);
        Assert.Equal(1, saved.Version);
        Assert.Empty(coordinator.State.GetPendingUids());
    }

    [Fact]
    public void Save_Success_DropsConflictedTaskAndKeepsSavedOutOfQueue()
    {
        var coordinator = new TaskSyncCoordinator();

        coordinator.EnqueueLocalChanges([NewTask("a", version: 0), NewTask("b", version: 0)]);
        var completed = coordinator.OnSaveSucceeded([NewTask("a", version: 1)]);

        var finished = SingleEffect(completed, TaskSyncEffectKind.SaveFinished);
        Assert.Equal(new[] { "b" }, Uids(finished.Conflicted));
        Assert.Equal(new[] { "a" }, finished.Saved.Select(version => version.Uid).ToArray());
        Assert.Empty(coordinator.State.GetPendingUids());
    }

    [Fact]
    public void Save_TransportError_RequeuesAndSchedulesFiveSecondRetry()
    {
        var coordinator = new TaskSyncCoordinator();

        var enqueued = coordinator.EnqueueLocalChanges([NewTask("1", version: 0)]);
        Assert.Equal(1, CountEffects(enqueued, TaskSyncEffectKind.SaveBatch));

        var failed = coordinator.OnSaveFailed();
        var retry = SingleEffect(failed, TaskSyncEffectKind.ScheduleRetry);
        Assert.Equal(TimeSpan.FromSeconds(5), retry.RetryDelay);
        Assert.Equal(TaskSyncCoordinator.RetryDelay, retry.RetryDelay);
        Assert.True(coordinator.State.Pending.ContainsKey("1"));
        Assert.True(coordinator.State.IsSaving);

        var resumed = coordinator.OnRetryElapsed();
        Assert.Equal(1, CountEffects(resumed, TaskSyncEffectKind.SaveBatch));

        coordinator.OnSaveSucceeded([NewTask("1", version: 1)]);
        Assert.Empty(coordinator.State.GetPendingUids());
        Assert.False(coordinator.State.IsSaving);
    }

    [Fact]
    public void Save_ReEditWhileInFlight_NotReportedConflictedUntilItSettles()
    {
        var coordinator = new TaskSyncCoordinator();

        coordinator.EnqueueLocalChanges([NewTask("1", version: 0)]);
        var reEdit = coordinator.EnqueueLocalChanges([NewTask("1", version: 0)]);
        Assert.Equal(0, CountEffects(reEdit, TaskSyncEffectKind.SaveBatch));

        var firstCycle = coordinator.OnSaveSucceeded([]);
        var firstFinished = SingleEffect(firstCycle, TaskSyncEffectKind.SaveFinished);
        Assert.Empty(firstFinished.Conflicted);
        Assert.Equal(1, CountEffects(firstCycle, TaskSyncEffectKind.SaveBatch));

        var secondCycle = coordinator.OnSaveSucceeded([]);
        var secondFinished = SingleEffect(secondCycle, TaskSyncEffectKind.SaveFinished);
        Assert.Equal(new[] { "1" }, Uids(secondFinished.Conflicted));
        Assert.Empty(coordinator.State.GetPendingUids());
    }

    [Fact]
    public void Save_DroppedConflict_RemovedFromInFlightMap()
    {
        var coordinator = new TaskSyncCoordinator();

        coordinator.EnqueueLocalChanges([NewTask("1", version: 0)]);
        coordinator.OnSaveSucceeded([]);

        Assert.Empty(coordinator.State.GetPendingUids());
        Assert.Empty(coordinator.State.InFlight);
        Assert.Empty(coordinator.State.Pending);
    }

    [Fact]
    public void GetPendingUids_ReturnsUniqueUidsFromBothQueues()
    {
        var coordinator = new TaskSyncCoordinator();

        coordinator.EnqueueLocalChanges([NewTask("a"), NewTask("b")]);
        coordinator.EnqueueLocalChanges([NewTask("b"), NewTask("c")]);

        var pendingUids = coordinator.State.GetPendingUids().OrderBy(uid => uid).ToList();
        Assert.Equal(new[] { "a", "b", "c" }, pendingUids);
    }

    [Fact]
    public void Enqueue_PersistsQueuedTasksToOutbox()
    {
        var coordinator = new TaskSyncCoordinator();

        var effects = coordinator.EnqueueLocalChanges([NewTask("1", version: 0)]);

        var persist = SingleEffect(effects, TaskSyncEffectKind.PersistOutbox);
        Assert.Equal(new[] { "1" }, Uids(persist.Tasks));
    }

    [Fact]
    public void Enqueue_PersistsOutboxBeforeDispatchingSave()
    {
        var coordinator = new TaskSyncCoordinator();

        var effects = coordinator.EnqueueLocalChanges([NewTask("1", version: 0)]);

        var persistIndex = IndexOfKind(effects, TaskSyncEffectKind.PersistOutbox);
        var saveIndex = IndexOfKind(effects, TaskSyncEffectKind.SaveBatch);
        Assert.InRange(persistIndex, 0, saveIndex - 1);
    }

    [Fact]
    public void Enqueue_WhileSaving_DoesNotStartASecondBatch()
    {
        var coordinator = new TaskSyncCoordinator();

        var first = coordinator.EnqueueLocalChanges([NewTask("a")]);
        var second = coordinator.EnqueueLocalChanges([NewTask("b")]);

        Assert.Equal(1, CountEffects(first, TaskSyncEffectKind.SaveBatch));
        Assert.Equal(0, CountEffects(second, TaskSyncEffectKind.SaveBatch));
        Assert.True(coordinator.State.InFlight.ContainsKey("a"));
        Assert.True(coordinator.State.Pending.ContainsKey("b"));
    }

    [Fact]
    public void Enqueue_SameUidTwice_KeepsTheLatestEdit()
    {
        var coordinator = new TaskSyncCoordinator();

        coordinator.EnqueueLocalChanges([NewTask("1", version: 0, title: "first")]);
        coordinator.EnqueueLocalChanges([NewTask("1", version: 0, title: "second")]);

        Assert.Equal("second", coordinator.State.Pending["1"].Title);
    }

    [Fact]
    public void Save_Success_PropagatesReturnedVersionIntoPendingReEdit()
    {
        var coordinator = new TaskSyncCoordinator();

        coordinator.EnqueueLocalChanges([NewTask("1", version: 0, title: "first")]);
        coordinator.EnqueueLocalChanges([NewTask("1", version: 0, title: "re-edit")]);

        var completed = coordinator.OnSaveSucceeded([NewTask("1", version: 5)]);

        var batch = SingleEffect(completed, TaskSyncEffectKind.SaveBatch);
        var sent = Assert.Single(batch.Tasks);
        Assert.Equal("1", sent.Uid);
        Assert.Equal(5, sent.Version);
        Assert.Equal("re-edit", sent.Title);
    }

    [Fact]
    public void Save_Success_SettlesFromRestResponseWithoutHubEcho()
    {
        var coordinator = new TaskSyncCoordinator();

        coordinator.EnqueueLocalChanges([NewTask("1", version: 0)]);
        var completed = coordinator.OnSaveSucceeded([NewTask("1", version: 1)]);

        Assert.False(coordinator.State.IsSaving);
        Assert.Empty(coordinator.State.Pending);
        Assert.Empty(coordinator.State.InFlight);
        Assert.Empty(coordinator.State.GetPendingUids());

        var finished = SingleEffect(completed, TaskSyncEffectKind.SaveFinished);
        var saved = Assert.Single(finished.Saved);
        Assert.Equal(1, saved.Version);
        var status = SingleEffect(completed, TaskSyncEffectKind.ReportSyncStatus);
        Assert.False(status.IsSaving);
    }

    [Fact]
    public void Reset_DuringFailedInFlightSave_PreservesPersistedOutbox()
    {
        var coordinator = new TaskSyncCoordinator();
        IReadOnlyList<TerminalTask> outbox = [];

        outbox = ApplyOutbox(outbox, coordinator.EnqueueLocalChanges([NewTask("1", version: 2)]));
        Assert.Equal(new[] { "1" }, Uids(outbox));

        coordinator.Reset();
        outbox = ApplyOutbox(outbox, coordinator.OnSaveFailed());
        Assert.Equal(new[] { "1" }, Uids(outbox));

        outbox = ApplyOutbox(outbox, coordinator.OnRetryElapsed());
        Assert.Equal(new[] { "1" }, Uids(outbox));
        Assert.Empty(coordinator.State.GetPendingUids());
    }

    [Fact]
    public void Save_Success_ClearsOutbox()
    {
        var coordinator = new TaskSyncCoordinator();
        IReadOnlyList<TerminalTask> outbox = [];

        outbox = ApplyOutbox(outbox, coordinator.EnqueueLocalChanges([NewTask("1", version: 0)]));
        outbox = ApplyOutbox(outbox, coordinator.OnSaveSucceeded([NewTask("1", version: 1)]));

        Assert.Empty(outbox);
    }

    [Fact]
    public void Save_TransportFailure_KeepsOutbox()
    {
        var coordinator = new TaskSyncCoordinator();
        IReadOnlyList<TerminalTask> outbox = [];

        outbox = ApplyOutbox(outbox, coordinator.EnqueueLocalChanges([NewTask("1", version: 0)]));
        outbox = ApplyOutbox(outbox, coordinator.OnSaveFailed());
        Assert.Equal(new[] { "1" }, Uids(outbox));

        outbox = ApplyOutbox(outbox, coordinator.OnRetryElapsed());
        outbox = ApplyOutbox(outbox, coordinator.OnSaveSucceeded([NewTask("1", version: 1)]));
        Assert.Empty(outbox);
    }

    [Fact]
    public void RestoreOutbox_RequeuesPersistedTasksAndReplaysThem()
    {
        var coordinator = new TaskSyncCoordinator();

        var effects = coordinator.RestoreOutbox([NewTask("1", version: 2)]);

        var batch = SingleEffect(effects, TaskSyncEffectKind.SaveBatch);
        Assert.Equal(new[] { "1" }, Uids(batch.Tasks));
        Assert.Equal(0, CountEffects(effects, TaskSyncEffectKind.PersistOutbox));

        var completed = coordinator.OnSaveSucceeded([NewTask("1", version: 3)]);
        var persist = SingleEffect(completed, TaskSyncEffectKind.PersistOutbox);
        Assert.Empty(persist.Tasks);
        Assert.Empty(coordinator.State.GetPendingUids());
    }

    [Fact]
    public void RestoreOutbox_Empty_DoesNothing()
    {
        var coordinator = new TaskSyncCoordinator();

        var effects = coordinator.RestoreOutbox([]);

        Assert.Empty(effects);
        Assert.False(coordinator.State.IsSaving);
    }

    [Fact]
    public void Reset_ClearsInMemoryQueuesButPreservesOutbox()
    {
        var coordinator = new TaskSyncCoordinator();
        IReadOnlyList<TerminalTask> outbox = [];

        outbox = ApplyOutbox(outbox, coordinator.EnqueueLocalChanges([NewTask("a")]));
        outbox = ApplyOutbox(outbox, coordinator.EnqueueLocalChanges([NewTask("b")]));
        var beforeReset = Uids(outbox).OrderBy(uid => uid).ToList();
        Assert.Equal(new[] { "a", "b" }, beforeReset);

        coordinator.Reset();

        Assert.Empty(coordinator.State.Pending);
        Assert.Empty(coordinator.State.InFlight);
        var afterReset = Uids(outbox).OrderBy(uid => uid).ToList();
        Assert.Equal(new[] { "a", "b" }, afterReset);
    }

    private static TerminalTask NewTask(string uid, int version = 0, string title = "Task")
    {
        return new TerminalTask
        {
            Uid = uid,
            Title = title,
            Date = null,
            Time = null,
            Order = 1,
            Completed = false,
            Deleted = false,
            Type = TerminalTaskType.Simple,
            IsProbable = false,
            Version = version,
        };
    }

    private static TaskSyncEffect SingleEffect(IReadOnlyList<TaskSyncEffect> effects, TaskSyncEffectKind kind)
    {
        return Assert.Single(effects, effect => effect.Kind == kind);
    }

    private static int CountEffects(IReadOnlyList<TaskSyncEffect> effects, TaskSyncEffectKind kind)
    {
        return effects.Count(effect => effect.Kind == kind);
    }

    private static int IndexOfKind(IReadOnlyList<TaskSyncEffect> effects, TaskSyncEffectKind kind)
    {
        for (var i = 0; i < effects.Count; i++)
        {
            if (effects[i].Kind == kind)
                return i;
        }

        return -1;
    }

    private static string[] Uids(IEnumerable<TerminalTask> tasks)
    {
        return tasks.Select(task => task.Uid).ToArray();
    }

    private static IReadOnlyList<TerminalTask> ApplyOutbox(
        IReadOnlyList<TerminalTask> store,
        IReadOnlyList<TaskSyncEffect> effects)
    {
        var persist = effects.LastOrDefault(effect => effect.Kind == TaskSyncEffectKind.PersistOutbox);
        return persist?.Tasks ?? store;
    }
}
