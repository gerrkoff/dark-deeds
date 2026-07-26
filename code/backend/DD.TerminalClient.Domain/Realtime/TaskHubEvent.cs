using DD.TerminalClient.Domain.Models;

namespace DD.TerminalClient.Domain.Realtime;

// A single real-time event produced by the hub client, carrying only domain types. An Update event
// carries the tasks pushed by the server (already mapped to terminal tasks); every other kind is a
// connectivity signal with no payload. Values are created through the static factories so the payload
// is meaningful only for Update, and the connectivity kinds reuse cached singletons.
public sealed record TaskHubEvent
{
    private static readonly IReadOnlyList<TerminalTask> NoTasks = [];

    private TaskHubEvent(TaskHubEventKind kind, IReadOnlyList<TerminalTask> tasks)
    {
        Kind = kind;
        Tasks = tasks;
    }

    public TaskHubEventKind Kind { get; }

    public IReadOnlyList<TerminalTask> Tasks { get; }

    public static TaskHubEvent Reconnecting { get; } = new(TaskHubEventKind.Reconnecting, NoTasks);

    public static TaskHubEvent Reconnected { get; } = new(TaskHubEventKind.Reconnected, NoTasks);

    public static TaskHubEvent Closed { get; } = new(TaskHubEventKind.Closed, NoTasks);

    public static TaskHubEvent Unauthorized { get; } = new(TaskHubEventKind.Unauthorized, NoTasks);

    public static TaskHubEvent Heartbeat { get; } = new(TaskHubEventKind.Heartbeat, NoTasks);

    public static TaskHubEvent Update(IReadOnlyList<TerminalTask> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        return new TaskHubEvent(TaskHubEventKind.Update, tasks);
    }
}
