using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Interface
{
    public interface IBotCommandParserService
    {
        Task<BotCommand> ParseCommand(string command, int chatId);
    }
}