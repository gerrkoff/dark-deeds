using DarkDeeds.BotIntegration.Dto;

namespace DarkDeeds.BotIntegration.Interface
{
    public interface IBotProcessMessageService
    {
        void ProcessMessage(UpdateDto update);
    }
}