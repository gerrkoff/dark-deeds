using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Implementation
{
    public class BotProcessShowTodoService : IBotProcessShowTodoService
    {
        private readonly IBotSendMessageService _botSendMessageService;

        public BotProcessShowTodoService(IBotSendMessageService botSendMessageService)
        {
            _botSendMessageService = botSendMessageService;
        }

        public void Process(ShowTodoCommand command)
        {
            _botSendMessageService.SendText(command.UserChatId, $"Show todo {command.Day}");
        }
    }
}