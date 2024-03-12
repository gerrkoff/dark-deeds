using DD.WebClientBff.Domain.Entities;

namespace DD.WebClientBff.Domain.Infrastructure;

public interface IUserSettingsRepository
{
    Task<UserSettingsEntity?> GetByIdAsync(string userId);

    Task UpsertAsync(UserSettingsEntity entity);
}
