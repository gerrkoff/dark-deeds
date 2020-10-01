using System.Threading.Tasks;
using DarkDeeds.Services.Dto;

namespace DarkDeeds.Services.Interface
{
    public interface ISettingsService
    {
        Task SaveAsync(SettingsDto settings, string userId);
        Task<SettingsDto> LoadAsync(string userId);
    }
}