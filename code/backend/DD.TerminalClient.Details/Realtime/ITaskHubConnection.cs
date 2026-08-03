using DD.Shared.Details.Abstractions.Dto;

namespace DD.TerminalClient.Details.Realtime;

// The seam that makes the hub client testable without a live SignalR endpoint. A single connection is
// created once and reused across reconnects: it can be started and stopped repeatedly, delivers server
// pushes and heartbeats through registered handlers, and reports every close (fault or intentional)
// through a single closed callback. The real implementation wraps a HubConnection; tests provide a fake.
internal interface ITaskHubConnection : IAsyncDisposable
{
    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    void OnUpdate(Action<IReadOnlyList<TaskDto>> handler);

    void OnHeartbeat(Action handler);

    void OnClosed(Func<Exception?, Task> handler);
}
