using DD.Shared.Data.Migrator.Models;
using DD.Shared.Data.Migrator.Services;
using DD.Shared.Details.Data;
using MongoDB.Driver;

namespace DD.Shared.Data.Migrator.Migrations;

internal sealed class D20240310N0_Init(IMongoDbContext context) : IMigrationBody
{
    public async Task UpAsync(CancellationToken cancellationToken)
    {
        var indexName = new CreateIndexModel<MigrationRecord>(
            Builders<MigrationRecord>.IndexKeys.Ascending(x => x.Name),
            new CreateIndexOptions { Unique = true });
        await context.GetCollection<MigrationRecord>(MigrationRunner.MigrationsCollectionName).Indexes
            .CreateOneAsync(indexName, cancellationToken: cancellationToken);
    }
}
