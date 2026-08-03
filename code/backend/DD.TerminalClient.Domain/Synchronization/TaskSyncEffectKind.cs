namespace DD.TerminalClient.Domain.Synchronization;

// The kinds of side effect the save state machine asks the application loop to perform. The loop
// switches on this to route each effect to the right host capability (disk, network, timer, UI),
// keeping TaskSyncCoordinator itself free of any I/O concern.
public enum TaskSyncEffectKind
{
    PersistOutbox,
    SaveBatch,
    ScheduleRetry,
    ReportSyncStatus,
    SaveFinished,
}
