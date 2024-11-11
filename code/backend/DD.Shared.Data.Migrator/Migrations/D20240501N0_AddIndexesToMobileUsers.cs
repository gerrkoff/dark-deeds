using DD.Clients.Details.MobileClient.Data;
using DD.MobileClient.Domain.Entities;
using DD.Shared.Data.Migrator.Models;
using MongoDB.Driver;

namespace DD.Shared.Data.Migrator.Migrations;

internal sealed class D20240501N0_AddIndexesToMobileUsers(
    MobileUserRepository mobileUsersRepository) : IMigrationBody
{
    public async Task UpAsync(CancellationToken cancellationToken)
    {
        var indexMobileUserUserId = new CreateIndexModel<MobileUserEntity>(
            Builders<MobileUserEntity>.IndexKeys.Ascending(x => x.MobileKey),
            new CreateIndexOptions { Unique = true, Sparse = true });
        await mobileUsersRepository.Collection.Indexes.CreateManyAsync(
            [
                indexMobileUserUserId
            ],
            cancellationToken);
    }
}
