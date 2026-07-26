using DD.TerminalClient.Domain.Models;

namespace DD.TerminalClient.Domain.Abstractions;

// The bearer-authenticated task transport. Load always requests the full snapshot from the current
// local Monday mapped to UTC midnight; save posts a batch and returns the server's authoritative copies
// (with incremented versions) so the caller can apply them immediately without waiting for a hub echo.
// Both speak in terminal tasks; the implementation maps to and from the shared transport contract.
public interface ITaskApiClient
{
    Task<IReadOnlyList<TerminalTask>> LoadTasksAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<TerminalTask>> SaveTasksAsync(
        IReadOnlyCollection<TerminalTask> tasks, CancellationToken cancellationToken);
}
