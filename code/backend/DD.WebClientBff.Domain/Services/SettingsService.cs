using AutoMapper;
using DD.Shared.Data.Abstractions;
using DD.WebClientBff.Domain.Dto;
using DD.WebClientBff.Domain.Entities;

namespace DD.WebClientBff.Domain.Services;

public interface ISettingsService
{
    Task SaveAsync(SettingsDto settings, string userId);
    Task<SettingsDto> LoadAsync(string userId);
}

class SettingsService(
    IRepository<SettingsEntity> settingsRepository,
    IMapper mapper)
    : ISettingsService
{
    public async Task SaveAsync(SettingsDto settings, string userId)
    {
        var entity = await FindUserSettings(userId);

        if (entity == null)
        {
            entity = mapper.Map<SettingsEntity>(settings);
            entity.UserId = userId;
        }
        else
        {
            entity.ShowCompleted = settings.ShowCompleted;
        }

        await settingsRepository.SaveAsync(entity);
    }

    public async Task<SettingsDto> LoadAsync(string userId)
    {
        var entity = await FindUserSettings(userId);

        if (entity == null)
            return new SettingsDto();

        return mapper.Map<SettingsDto>(entity);
    }

    private Task<SettingsEntity?> FindUserSettings(string userId)
    {
        var result = settingsRepository.GetAll().FirstOrDefault(x => string.Equals(x.UserId, userId));
        return Task.FromResult(result);
    }
}
