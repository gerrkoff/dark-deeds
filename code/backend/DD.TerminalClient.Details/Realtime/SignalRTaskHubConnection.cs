using DD.Shared.Details.Abstractions.Dto;
using Microsoft.AspNetCore.SignalR.Client;

namespace DD.TerminalClient.Details.Realtime;

// The real adapter over a SignalR HubConnection. It only forwards: subscribing the update and heartbeat
// handlers to the exact event names, exposing the connection's closed callback, and delegating start,
// stop and disposal. All reconnect, buffering and event-translation logic lives in TaskHubClient, so
// this type stays thin enough not to need its own tests.
internal sealed class SignalRTaskHubConnection(HubConnection connection) : ITaskHubConnection
{
    private readonly HubConnection _connection = connection ?? throw new ArgumentNullException(nameof(connection));

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _connection.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _connection.StopAsync(cancellationToken);
    }

    public void OnUpdate(Action<IReadOnlyList<TaskDto>> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _connection.On<List<TaskDto>>(TaskHubProtocol.UpdateEventName, tasks => handler(tasks));
    }

    public void OnHeartbeat(Action handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _connection.On(TaskHubProtocol.HeartbeatEventName, handler);
    }

    public void OnClosed(Func<Exception?, Task> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _connection.Closed += handler;
    }

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }
}
