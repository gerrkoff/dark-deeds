using System.Threading.Tasks;

namespace DarkDeeds.TelegramClientApp.Services.Interface
{
    public interface IBotSendMessageService
    {
        Task SendUnknownCommandAsync(int userChatId);
        Task SendTextAsync(int userChatId, string text);
        Task SendFailedAsync(int userChatId);
    }
}