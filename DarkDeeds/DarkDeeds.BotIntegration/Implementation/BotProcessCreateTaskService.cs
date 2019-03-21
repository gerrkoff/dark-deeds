using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.BotIntegration.Implementation
{
    public class BotProcessCreateTaskService : IBotProcessCreateTaskService
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly ITelegramService _telegramService;

        public BotProcessCreateTaskService(IBotSendMessageService botSendMessageService, ITelegramService telegramService)
        {
            _botSendMessageService = botSendMessageService;
            _telegramService = telegramService;
        }

        public async Task ProcessAsync(CreateTaskCommand command)
        {
            string userId = await _telegramService.GetUserId(command.UserChatId);
            await _botSendMessageService.SendTextAsync(command.UserChatId, $"Create task: {command.Task.Title}");
        }
    }
}