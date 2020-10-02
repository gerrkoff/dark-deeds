using System.Threading.Tasks;
using DarkDeeds.Models.Dto;

namespace DarkDeeds.Services.Interface
{
    public interface ISettingsService
    {
        Task SaveAsync(SettingsDto settings, string userId);
        Task<SettingsDto> LoadAsync(string userId);
    }
}