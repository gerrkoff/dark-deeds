using DarkDeeds.Models.Telegram;

namespace DarkDeeds.Services.Interface
{
    public interface IBotProcessMessageService
    {
        void ProcessMessage(UpdateDto update);
    }
}