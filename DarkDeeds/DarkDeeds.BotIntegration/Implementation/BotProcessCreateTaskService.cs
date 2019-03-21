using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Implementation
{
    public class BotProcessCreateTaskService : IBotProcessCreateTaskService
    {
        private readonly IBotSendMessageService _botSendMessageService;

        public BotProcessCreateTaskService(IBotSendMessageService botSendMessageService)
        {
            _botSendMessageService = botSendMessageService;
        }

        public void Process(CreateTaskCommand command)
        {
            _botSendMessageService.SendText($"Create task: {command.Task.Title}");
        }
    }
}