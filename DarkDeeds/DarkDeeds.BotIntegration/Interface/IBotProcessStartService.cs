using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Interface
{
    public interface IBotProcessStartService
    {
        void Process(StartCommand command);
    }
}