using System.Threading.Tasks;
using DarkDeeds.WebClientBff.Services.Dto;

namespace DarkDeeds.WebClientBff.Services.Services.Interface
{
    public interface ISettingsService
    {
        Task SaveAsync(SettingsDto settings, string userId);
        Task<SettingsDto> LoadAsync(string userId);
    }
}