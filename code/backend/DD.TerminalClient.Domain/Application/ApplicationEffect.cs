using DD.TerminalClient.Domain.Models;

namespace DD.TerminalClient.Domain.Application;

// The high-level side effects the pure reducer asks the event loop to run. The reducer never performs
// I/O: it returns these and the loop routes each to the right collaborator (the save coordinator, the
// hub, the auth client, the profile store). Quitting is carried on the state flag, not an effect.
public enum ApplicationEffectKind
{
    EnqueueTaskChanges,
    SignIn,
    Reconnect,
    AcceptDataReset,
}

// One effect with only the payload its Kind needs. EnqueueTaskChanges carries the minimal renumbered
// batch to persist and save; SignIn carries the collected credentials; Reconnect and AcceptDataReset
// are payload-free singletons.
public sealed record ApplicationEffect
{
    private static readonly IReadOnlyList<TerminalTask> NoTasks = [];

    public ApplicationEffectKind Kind { get; private init; }

    public IReadOnlyList<TerminalTask> Tasks { get; private init; } = NoTasks;

    public string Username { get; private init; } = string.Empty;

    public string Password { get; private init; } = string.Empty;

    public static ApplicationEffect Reconnect { get; } = new() { Kind = ApplicationEffectKind.Reconnect };

    public static ApplicationEffect AcceptDataReset { get; } =
        new() { Kind = ApplicationEffectKind.AcceptDataReset };

    public static ApplicationEffect EnqueueTaskChanges(IReadOnlyList<TerminalTask> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        return new ApplicationEffect { Kind = ApplicationEffectKind.EnqueueTaskChanges, Tasks = tasks };
    }

    public static ApplicationEffect SignIn(string username, string password)
    {
        return new ApplicationEffect
        {
            Kind = ApplicationEffectKind.SignIn,
            Username = username,
            Password = password,
        };
    }
}
