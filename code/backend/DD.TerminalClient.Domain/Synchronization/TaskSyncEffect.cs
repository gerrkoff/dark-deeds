using DD.TerminalClient.Domain.Models;

namespace DD.TerminalClient.Domain.Synchronization;

// A single side effect produced by the save state machine, carrying only the payload its Kind needs.
// TaskSyncCoordinator stays pure and synchronous: it mutates its in-memory maps and returns these
// values, and the application loop performs the I/O (disk, network, timer, UI) and reports results
// back. Values are created through the static factories so only the fields meaningful for a given
// Kind are set; the loop switches on Kind and reads the matching payload.
public sealed record TaskSyncEffect
{
    private static readonly IReadOnlyList<TerminalTask> NoTasks = [];
    private static readonly IReadOnlyList<TerminalTaskVersion> NoVersions = [];

    private TaskSyncEffect(TaskSyncEffectKind kind)
    {
        Kind = kind;
    }

    public TaskSyncEffectKind Kind { get; }

    // For PersistOutbox the tasks to persist as the durable outbox (empty means clear it); for
    // SaveBatch the batch to send to the backend. Empty for every other kind.
    public IReadOnlyList<TerminalTask> Tasks { get; private init; } = NoTasks;

    // For ScheduleRetry, how long to wait before calling OnRetryElapsed. Zero for every other kind.
    public TimeSpan RetryDelay { get; private init; }

    // For ReportSyncStatus, whether a save is in progress.
    public bool IsSaving { get; private init; }

    // For SaveFinished, the batch size on a transport failure and zero on success.
    public int NotSaved { get; private init; }

    // For SaveFinished, the accepted (Uid, Version) pairs to apply to the cache. Empty otherwise.
    public IReadOnlyList<TerminalTaskVersion> Saved { get; private init; } = NoVersions;

    // For SaveFinished, the in-flight tasks the backend rejected on a version conflict with no pending
    // re-edit; their edit is dropped and reconciled later. Empty otherwise.
    public IReadOnlyList<TerminalTask> Conflicted { get; private init; } = NoTasks;

    public static TaskSyncEffect PersistOutbox(IReadOnlyList<TerminalTask> outbox)
    {
        ArgumentNullException.ThrowIfNull(outbox);
        return new TaskSyncEffect(TaskSyncEffectKind.PersistOutbox) { Tasks = outbox };
    }

    public static TaskSyncEffect SaveBatch(IReadOnlyList<TerminalTask> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        return new TaskSyncEffect(TaskSyncEffectKind.SaveBatch) { Tasks = tasks };
    }

    public static TaskSyncEffect ScheduleRetry(TimeSpan delay)
    {
        return new TaskSyncEffect(TaskSyncEffectKind.ScheduleRetry) { RetryDelay = delay };
    }

    public static TaskSyncEffect ReportSyncStatus(bool isSaving)
    {
        return new TaskSyncEffect(TaskSyncEffectKind.ReportSyncStatus) { IsSaving = isSaving };
    }

    public static TaskSyncEffect SaveFinished(
        int notSaved,
        IReadOnlyList<TerminalTaskVersion> saved,
        IReadOnlyList<TerminalTask> conflicted)
    {
        ArgumentNullException.ThrowIfNull(saved);
        ArgumentNullException.ThrowIfNull(conflicted);
        return new TaskSyncEffect(TaskSyncEffectKind.SaveFinished)
        {
            NotSaved = notSaved,
            Saved = saved,
            Conflicted = conflicted,
        };
    }
}
