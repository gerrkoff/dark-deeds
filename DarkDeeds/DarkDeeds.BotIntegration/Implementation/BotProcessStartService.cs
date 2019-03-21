using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.BotIntegration.Implementation
{
    public class BotProcessStartService : IBotProcessStartService
    {
        private readonly ITelegramService _telegramService;

        public BotProcessStartService(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task ProcessAsync(StartCommand command)
        {
            await _telegramService.UpdateChatId(command.UserChatKey, command.UserChatId);
        }
    }
}