using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp.Dto;

namespace DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp
{
    public interface ITelegramClientApp
    {
        Task<TelegramStartDto> Start(int timezoneOffset);
    }
}
