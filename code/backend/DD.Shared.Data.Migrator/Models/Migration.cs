namespace DD.Shared.Data.Migrator.Models;

internal sealed class Migration
{
    public required IMigrationBody Body { get; init; }

    public required string Name { get; init; }
}
