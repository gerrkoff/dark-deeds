using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.Common.Extensions;
using DarkDeeds.Data.Entity;
using DarkDeeds.Data.Repository;
using DarkDeeds.Models;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class SettingsService : ISettingsService
    {
        private readonly IRepositoryNonDeletable<SettingsEntity> _settingsRepository;

        public SettingsService(IRepositoryNonDeletable<SettingsEntity> settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public async Task SaveAsync(SettingsDto settings, string userId)
        {
            SettingsEntity entity = await FindUserSettings(userId);

            if (entity == null)
            {
                entity = Mapper.Map<SettingsEntity>(settings);
                entity.UserId = userId;
            }
            else
            {
                entity.ShowCompleted = settings.ShowCompleted;                
            }
            
            await _settingsRepository.SaveAsync(entity);
        }

        public async Task<SettingsDto> LoadAsync(string userId)
        {
            SettingsEntity entity = await FindUserSettings(userId);

            if (entity == null)
                return new SettingsDto();

            return Mapper.Map<SettingsDto>(entity);
        }

        private Task<SettingsEntity> FindUserSettings(string userId) => _settingsRepository.GetAll()
            .FirstOrDefaultSafeAsync(x => string.Equals(x.UserId, userId));
    }
}