using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.WebClientBffApp.Entities;
using DarkDeeds.WebClientBffApp.Infrastructure.Data;
using DarkDeeds.WebClientBffApp.Services.Dto;
using DarkDeeds.WebClientBffApp.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.WebClientBffApp.Services.Implementation
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
            .FirstOrDefaultAsync(x => string.Equals(x.UserId, userId));
    }
}