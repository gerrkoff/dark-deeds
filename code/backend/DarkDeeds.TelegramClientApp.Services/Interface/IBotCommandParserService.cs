using System.Threading.Tasks;
using DarkDeeds.TelegramClientApp.Services.Models.Commands;

namespace DarkDeeds.TelegramClientApp.Services.Interface
{
    public interface IBotCommandParserService
    {
        Task<BotCommand> ParseCommand(string command, int chatId);
    }
}