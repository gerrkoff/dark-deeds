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
            var settingsEntity = Mapper.Map<SettingsEntity>(settings);
            settingsEntity.UserId = userId;

            await _settingsRepository.SaveAsync(settingsEntity);
        }

        public async Task<SettingsDto> LoadAsync(string userId)
        {
            SettingsEntity settings = await _settingsRepository.GetAll()
                .FirstOrDefaultSafeAsync(x => string.Equals(x.UserId, userId));

            if (settings == null)
                return new SettingsDto();

            return Mapper.Map<SettingsDto>(settings);
        }
    }
}