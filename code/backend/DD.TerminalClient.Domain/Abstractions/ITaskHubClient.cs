using DD.TerminalClient.Domain.Realtime;

namespace DD.TerminalClient.Domain.Abstractions;

// The real-time task channel. It owns one hub connection and its automatic reconnect loop, translating
// every raw connectivity condition into TaskHubEvent values delivered to the application's event sink;
// it never touches application or UI state directly. While a snapshot load is in progress the client
// buffers incoming server pushes in arrival order, and the application drains them only after it has
// reconciled the snapshot, so a push can never be applied before the snapshot it belongs after.
public interface ITaskHubClient : IAsyncDisposable
{
    // Bring the connection up and keep it up. Returns once the first connect attempt has completed
    // (success, scheduled retry, or unauthorized); it never blocks on connectivity.
    Task StartAsync(CancellationToken cancellationToken);

    // Take the connection down and stop the reconnect loop; no further reconnect is attempted.
    Task StopAsync(CancellationToken cancellationToken);

    // Force an immediate reconnect, bypassing any backoff wait, so the application can reload the full
    // snapshot after a manual refresh.
    Task ReconnectNowAsync(CancellationToken cancellationToken);

    // Leave buffering mode and return the buffered Update events in arrival order for post-reconciliation
    // replay. Subsequent server pushes flow straight to the event sink until buffering re-opens on the
    // next reconnect.
    IReadOnlyList<TaskHubEvent> DrainBufferedUpdates();
}
