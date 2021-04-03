using System.Threading.Tasks;
using DarkDeeds.Infrastructure.Communication.TelegramClientApp.Dto;

namespace DarkDeeds.Infrastructure.Communication.TelegramClientApp
{
    public interface ITelegramClientApp
    {
        Task<TelegramStartDto> Start(int timezoneOffset);
    }
}
