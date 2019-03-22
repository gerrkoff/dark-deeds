using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.BotIntegration.Implementation
{
    public class BotProcessStartService : IBotProcessStartService
    {
        private readonly ITelegramService _telegramService;
        private readonly IBotSendMessageService _botSendMessageService;

        public BotProcessStartService(ITelegramService telegramService, IBotSendMessageService botSendMessageService)
        {
            _telegramService = telegramService;
            _botSendMessageService = botSendMessageService;
        }

        public async Task ProcessAsync(StartCommand command)
        {
            try
            {
                await _telegramService.UpdateChatId(command.UserChatKey, command.UserChatId);
                await _botSendMessageService.SendTextAsync(command.UserChatId, "Registered");
            }
            catch
            {
                await _botSendMessageService.SendFailedAsync(command.UserChatId);
                throw;
            }
        }
    }
}