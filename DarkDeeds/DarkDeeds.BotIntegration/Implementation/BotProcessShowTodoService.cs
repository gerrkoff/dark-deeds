using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.BotIntegration.Implementation
{
    public class BotProcessShowTodoService : IBotProcessShowTodoService
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly ITelegramService _telegramService;

        public BotProcessShowTodoService(IBotSendMessageService botSendMessageService, ITelegramService telegramService)
        {
            _botSendMessageService = botSendMessageService;
            _telegramService = telegramService;
        }

        public async Task ProcessAsync(ShowTodoCommand command)
        {
            string userId = await _telegramService.GetUserId(command.UserChatId);
            await _botSendMessageService.SendTextAsync(command.UserChatId, $"Show todo {command.Day}");
        }
    }
}