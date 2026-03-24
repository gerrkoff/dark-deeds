using System.Diagnostics.CodeAnalysis;
using DD.WebClientBff.Domain.Dto;
using DD.WebClientBff.Domain.Entities;
using DD.WebClientBff.Domain.Infrastructure;

namespace DD.WebClientBff.Domain.Services;

public interface IUserSettingsService
{
    Task SaveAsync(UserSettingsDto userSettings, string userId);

    Task<UserSettingsDto> LoadAsync(string userId);
}

internal sealed class UserSettingsService(
    IUserSettingsRepository settingsRepository)
    : IUserSettingsService
{
    public async Task SaveAsync(UserSettingsDto userSettings, string userId)
    {
        var entity = await FindUserSettings(userId);

        if (entity == null)
        {
            entity = new UserSettingsEntity { ShowCompleted = userSettings.ShowCompleted, UserId = userId };
        }
        else
        {
            entity.ShowCompleted = userSettings.ShowCompleted;
        }

        await settingsRepository.UpsertAsync(entity);
    }

    public async Task<UserSettingsDto> LoadAsync(string userId)
    {
        var entity = await FindUserSettings(userId);

        return entity == null
            ? new UserSettingsDto()
            : new UserSettingsDto { ShowCompleted = entity.ShowCompleted };
    }

    [SuppressMessage("Globalization", "CA1309:Use ordinal string comparison", Justification = "IQueryable")]
    private async Task<UserSettingsEntity?> FindUserSettings(string userId)
    {
        var result = await settingsRepository.GetByIdAsync(userId);
        return result;
    }
}
