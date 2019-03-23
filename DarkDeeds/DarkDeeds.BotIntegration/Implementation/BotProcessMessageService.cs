using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Dto;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Interface.CommandProcessor;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Models;

namespace DarkDeeds.BotIntegration.Implementation
{
    public class BotProcessMessageService : IBotProcessMessageService
    {
        private readonly IBotSendMessageService _botSendMessageService;
        private readonly IBotCommandParserService _botCommandParserService;
        
        private readonly IShowTodoCommandProcessor _showTodoCommandProcessor;
        private readonly ICreateTaskCommandProcessor _createTaskCommandProcessor;
        private readonly IStartCommandProcessor _startCommandProcessor;

        public BotProcessMessageService(
            IBotSendMessageService botSendMessageService,
            IBotCommandParserService botCommandParserService,
            IShowTodoCommandProcessor showTodoCommandProcessor, 
            ICreateTaskCommandProcessor createTaskCommandProcessor,
            IStartCommandProcessor startCommandProcessor)
        {
            _botSendMessageService = botSendMessageService;
            _botCommandParserService = botCommandParserService;
            _showTodoCommandProcessor = showTodoCommandProcessor;
            _createTaskCommandProcessor = createTaskCommandProcessor;
            _startCommandProcessor = startCommandProcessor;
        }

        public Task ProcessMessageAsync(UpdateDto update, Action<IEnumerable<TaskDto>> sendUpdateTasks)
        {
            string text = update.Message.Text.Trim();
            int userChatId = update.Message.Chat.Id;

            BotCommand command = _botCommandParserService.ParseCommand(text);
            if (command != null)
                command.UserChatId = userChatId;

            if (command is ShowTodoCommand showTodoCommand)
                return _showTodoCommandProcessor.ProcessAsync(showTodoCommand);
            
            if (command is StartCommand startCommand)
                return _startCommandProcessor.ProcessAsync(startCommand);
            
            if (command is CreateTaskCommand createTaskCommand)
            {
                _createTaskCommandProcessor.BindSendUpdateTasks(sendUpdateTasks);
                return _createTaskCommandProcessor.ProcessAsync(createTaskCommand);
            }

            return _botSendMessageService.SendUnknownCommandAsync(userChatId);
        }
    }
}