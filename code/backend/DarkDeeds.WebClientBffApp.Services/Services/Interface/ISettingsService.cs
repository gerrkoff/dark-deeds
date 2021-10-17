using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Services.Dto;

namespace DarkDeeds.WebClientBffApp.Services.Services.Interface
{
    public interface ISettingsService
    {
        Task SaveAsync(SettingsDto settings, string userId);
        Task<SettingsDto> LoadAsync(string userId);
    }
}