namespace DD.TerminalClient.Domain.Models;

// The terminal client's single task value, used everywhere: the hydrated task cache, the Overview
// projection and the durable outbox. A persisted outbox entry is exactly this value, keyed by Uid.
// Dates are timezone-free DateOnly (null = No Date); Time is minutes from midnight (null = unset);
// Version carries the backend optimistic-concurrency token. Immutable: every edit yields a copy.
public sealed record TerminalTask
{
    public required string Uid { get; init; }

    public required string Title { get; init; }

    public DateOnly? Date { get; init; }

    public int? Time { get; init; }

    public int Order { get; init; }

    public bool Completed { get; init; }

    public bool Deleted { get; init; }

    public TerminalTaskType Type { get; init; }

    public bool IsProbable { get; init; }

    public int Version { get; init; }
}
