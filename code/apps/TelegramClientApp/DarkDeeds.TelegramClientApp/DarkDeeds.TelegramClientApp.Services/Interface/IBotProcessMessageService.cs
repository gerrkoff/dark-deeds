using System.Threading.Tasks;
using DarkDeeds.TelegramClientApp.Services.Dto;

namespace DarkDeeds.TelegramClientApp.Services.Interface
{
    public interface IBotProcessMessageService
    {
        Task ProcessMessageAsync(UpdateDto update);
    }
}