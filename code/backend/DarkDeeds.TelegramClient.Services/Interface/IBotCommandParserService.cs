using System.Threading.Tasks;
using DarkDeeds.TelegramClient.Services.Models.Commands;

namespace DarkDeeds.TelegramClient.Services.Interface
{
    public interface IBotCommandParserService
    {
        Task<BotCommand> ParseCommand(string command, int chatId);
    }
}