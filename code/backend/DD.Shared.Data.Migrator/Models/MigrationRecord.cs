namespace DD.Shared.Data.Migrator.Models;

internal sealed class MigrationRecord
{
    public required string Name { get; init; }

    public required DateTime AppliedAt { get; init; }
}
