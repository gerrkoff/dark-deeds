using System.Linq.Expressions;
using DD.MobileClient.Domain.Entities;
using DD.MobileClient.Domain.Infrastructure;
using DD.Shared.Data;
using MongoDB.Driver;

namespace DD.MobileClient.Details.Data;

public sealed class MobileUserRepository(IMongoDbContext dbContext)
    : BaseRepository<MobileUserEntity>(dbContext, "mobileUsers"), IMobileUserRepository
{
    static MobileUserRepository()
    {
        RegisterDefaultMap<MobileUserEntity>();
    }

    protected override Expression<Func<MobileUserEntity, string>> FieldId => x => x.MobileKey;

    public async Task<MobileUserEntity?> GetByMobileKeyAsync(string mobileKey)
    {
        using var cursor = await Collection
            .FindAsync(x => x.MobileKey == mobileKey);
        return await cursor.SingleOrDefaultAsync();
    }
}
