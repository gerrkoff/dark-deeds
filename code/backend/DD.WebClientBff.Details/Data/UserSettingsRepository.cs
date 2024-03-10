using System.Linq.Expressions;
using DD.Shared.Data;
using DD.WebClientBff.Domain.Entities;
using DD.WebClientBff.Domain.Infrastructure;

namespace DD.WebClientBff.Details.Data;

public sealed class UserSettingsRepository(IMongoDbContext dbContext)
    : BaseRepository<UserSettingsEntity>(dbContext, "userSettings"), IUserSettingsRepository
{
    static UserSettingsRepository()
    {
        RegisterDefaultMap<UserSettingsEntity>();
    }

    protected override Expression<Func<UserSettingsEntity, string>> FieldId => x => x.UserId;
}
