using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.Backend.Entities;
using DarkDeeds.WebClientBff.Services.Dto;
using DarkDeeds.WebClientBff.Services.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.WebClientBff.Services.Services.Implementation
{
    public class SettingsService : ISettingsService
    {
        private readonly IRepositoryNonDeletable<SettingsEntity> _settingsRepository;
        private readonly IMapper _mapper;

        public SettingsService(IRepositoryNonDeletable<SettingsEntity> settingsRepository, IMapper mapper)
        {
            _settingsRepository = settingsRepository;
            _mapper = mapper;
        }

        public async Task SaveAsync(SettingsDto settings, string userId)
        {
            SettingsEntity entity = await FindUserSettings(userId);

            if (entity == null)
            {
                entity = _mapper.Map<SettingsEntity>(settings);
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

            return _mapper.Map<SettingsDto>(entity);
        }

        private Task<SettingsEntity> FindUserSettings(string userId) => _settingsRepository.GetAll()
            .FirstOrDefaultAsync(x => string.Equals(x.UserId, userId));
    }
}