using DarkDeeds.BotIntegration.Dto;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Implementation
{
    public class BotProcessMessageService : IBotProcessMessageService
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly IBotCommandParserService _botCommandParserService;
        private readonly IBotProcessShowTodoService _botProcessShowTodoService;
        private readonly IBotProcessCreateTaskService _botProcessCreateTaskService;

        public BotProcessMessageService(
            IBotSendMessageService botSendMessageService,
            IBotCommandParserService botCommandParserService,
            IBotProcessShowTodoService botProcessShowTodoService, 
            IBotProcessCreateTaskService botProcessCreateTaskService)
        {
            _botSendMessageService = botSendMessageService;
            _botCommandParserService = botCommandParserService;
            _botProcessShowTodoService = botProcessShowTodoService;
            _botProcessCreateTaskService = botProcessCreateTaskService;
        }

        public void ProcessMessage(UpdateDto update)
        {
            string text = update.Message.Text.Trim();

            BotCommand command = _botCommandParserService.ParseCommand(text);

            if (command is ShowTodoCommand showTodoCommand)
            {
                _botProcessShowTodoService.Process(showTodoCommand);
                return;
            }

            if (command is CreateTaskCommand createTaskCommand)
            {
                _botProcessCreateTaskService.Process(createTaskCommand);
                return;
            }

            _botSendMessageService.SendUnknownCommand();
        }
    }
}