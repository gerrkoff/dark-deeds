namespace DD.TerminalClient.Domain.Realtime;

// The kinds of real-time condition the hub client translates into pure domain events. Everything the
// SignalR connection reports - server pushes, liveness pings and connectivity transitions - collapses
// into one of these, so the application loop reacts to intent rather than to raw connection callbacks.
public enum TaskHubEventKind
{
    Update,
    Heartbeat,
    Reconnecting,
    Reconnected,
    Closed,
    Unauthorized,
}
