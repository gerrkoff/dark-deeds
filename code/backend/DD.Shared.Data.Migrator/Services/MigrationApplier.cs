using DD.Shared.Data.Migrator.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;

namespace DD.Shared.Data.Migrator.Services;

internal sealed class MigrationApplier(
    ILogger<MigrationApplier> logger,
    IMigratorMongoDbContext context)
{
    static MigrationApplier()
    {
        BsonClassMap.RegisterClassMap<MigrationRecord>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
        });
    }

    public async Task ApplyMigrationAsync(Migration migration, CancellationToken cancellationToken)
    {
        Log.ApplyingMigration(logger, migration.Name);

        await migration.Body.UpAsync(cancellationToken);
        var record = new MigrationRecord
        {
            Name = migration.Name,
            AppliedAt = DateTime.UtcNow,
        };
        await context
            .GetCollection<MigrationRecord>(MigrationRunner.MigrationsCollectionName)
            .InsertOneAsync(record, cancellationToken: cancellationToken);

        Log.AppliedMigration(logger, migration.Name);
    }
}
