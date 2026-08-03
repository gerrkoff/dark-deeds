namespace DD.TerminalClient.Domain.Models;

// The identity/version pair the server returns for an accepted save. Mirrors the frontend
// TaskVersionModel: after a successful save only the optimistic-concurrency Version is propagated
// back into the cached task, never the full server copy, so a concurrent in-flight re-edit is not
// reverted by the save that preceded it.
public sealed record TerminalTaskVersion(string Uid, int Version);
