using System.Threading.Tasks;
using DarkDeeds.WebClientBff.Infrastructure.Communication.TelegramClientApp.Dto;

namespace DarkDeeds.WebClientBff.Infrastructure.Communication.TelegramClientApp
{
    public interface ITelegramClientApp
    {
        Task<TelegramStartDto> Start(int timezoneOffset);
    }
}
