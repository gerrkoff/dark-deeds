using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Interface
{
    public interface IBotCommandParserService
    {
        BotCommand ParseCommand(string command);
    }
}