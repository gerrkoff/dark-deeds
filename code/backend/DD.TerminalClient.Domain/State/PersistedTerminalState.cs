using DD.TerminalClient.Domain.Models;

namespace DD.TerminalClient.Domain.State;

// The complete on-disk snapshot of one profile's local state: which user owns the cache/outbox, the
// hydrated task cache, the durable outbox of unsaved edits, and the local completed-visibility toggle.
// It is schema-versioned so future layout changes migrate forward without losing the outbox. Immutable:
// every change produces a copy, and the store always stamps CurrentSchemaVersion when it writes.
public sealed record PersistedTerminalState
{
    // Bump when the persisted layout changes and add a matching migration so older files upgrade
    // in place. Schema 0 denotes pre-versioning documents that predate this field entirely.
    public const int CurrentSchemaVersion = 1;

    public static PersistedTerminalState Empty { get; } = new();

    public int SchemaVersion { get; init; } = CurrentSchemaVersion;

    // The username (data owner) whose cache/outbox this file holds. Null before the first successful
    // login. The startup owner guard clears user state when a different user signs into the profile.
    public string? DataOwner { get; init; }

    public IReadOnlyList<TerminalTask> CachedTasks { get; init; } = [];

    public IReadOnlyList<TerminalTask> Outbox { get; init; } = [];

    public bool ShowCompleted { get; init; }
}
