using DD.Shared.Data.Migrator.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace DD.Shared.Data.Migrator.Services;

internal class MigrationProvider(
    ILogger<MigrationProvider> logger,
    IMigratorMongoDbContext context,
    IEnumerable<IMigrationBody> migrationBodies)
{
    public IReadOnlyList<Migration> GetMigrationsToApply()
    {
        var migrations = LoadMigrations(migrationBodies);
        var appliedMigrations = GetAppliedMigrations();
        var migrationsToApply = new List<Migration>();

        foreach (var migration in migrations)
        {
            if (appliedMigrations.Contains(migration.Name))
            {
                Log.SkippedMigration(logger, migration.Name);
            }
            else
            {
                migrationsToApply.Add(migration);
                Log.FoundMigrationToApply(logger, migration.Name);
            }
        }

        return migrationsToApply;
    }

    private HashSet<string> GetAppliedMigrations()
    {
        var collection = context.GetCollection<MigrationRecord>(MigrationRunner.MigrationsCollectionName);
        var migrations = collection.Find(FilterDefinition<MigrationRecord>.Empty).ToList();
        return migrations.Select(x => x.Name).ToHashSet();
    }

    private static List<Migration> LoadMigrations(IEnumerable<IMigrationBody> migrationBodies)
    {
        var migrations = new List<Migration>();

        foreach (var migrationBody in migrationBodies)
        {
            var migrationName = migrationBody.GetType().Name;
            var migration = new Migration
            {
                Body = migrationBody,
                Name = migrationName,
            };
            migrations.Add(migration);
        }

        return migrations;
    }
}
