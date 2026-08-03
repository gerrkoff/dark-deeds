using DD.TerminalClient.Domain.State;

namespace DD.TerminalClient.Domain.Abstractions;

// Atomic, per-profile persistence of the terminal client's local state (data owner, cached tasks,
// durable outbox, completed visibility). Writes must be crash-safe: the previous good file survives a
// failed save. Load returns null on first run and throws TerminalStateException for malformed or
// unsupported state so startup can block with an actionable message instead of losing the outbox.
public interface ILocalStateStore
{
    PersistedTerminalState? Load();

    void Save(PersistedTerminalState state);
}
