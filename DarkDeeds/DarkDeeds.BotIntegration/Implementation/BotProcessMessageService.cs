using System.Threading.Tasks;
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
        private readonly IBotProcessStartService _botProcessStartService;

        public BotProcessMessageService(
            IBotSendMessageService botSendMessageService,
            IBotCommandParserService botCommandParserService,
            IBotProcessShowTodoService botProcessShowTodoService, 
            IBotProcessCreateTaskService botProcessCreateTaskService,
            IBotProcessStartService botProcessStartService)
        {
            _botSendMessageService = botSendMessageService;
            _botCommandParserService = botCommandParserService;
            _botProcessShowTodoService = botProcessShowTodoService;
            _botProcessCreateTaskService = botProcessCreateTaskService;
            _botProcessStartService = botProcessStartService;
        }

        public Task ProcessMessageAsync(UpdateDto update)
        {
            string text = update.Message.Text.Trim();
            int userChatId = update.Message.Chat.Id;

            BotCommand command = _botCommandParserService.ParseCommand(text);
            if (command != null)
                command.UserChatId = userChatId;

            if (command is ShowTodoCommand showTodoCommand)
                return _botProcessShowTodoService.ProcessAsync(showTodoCommand);

            if (command is CreateTaskCommand createTaskCommand)
                return _botProcessCreateTaskService.ProcessAsync(createTaskCommand);
            
            if (command is StartCommand startCommand)
                return _botProcessStartService.ProcessAsync(startCommand);

            return _botSendMessageService.SendUnknownCommandAsync(userChatId);
        }
    }
}