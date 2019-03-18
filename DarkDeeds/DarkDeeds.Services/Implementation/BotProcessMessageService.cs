using DarkDeeds.Models.Telegram;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class BotProcessMessageService : IBotProcessMessageService
    {
        private readonly IBotSendMessageService _botSendMessageService;

        public BotProcessMessageService(IBotSendMessageService botSendMessageService)
        {
            _botSendMessageService = botSendMessageService;
        }

        public void ProcessMessage(UpdateDto update)
        {
        }
    }
}