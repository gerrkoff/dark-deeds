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

        public async Task ProcessMessageAsync(UpdateDto update)
        {
            string text = update.Message.Text.Trim();
            int userChatId = update.Message.Chat.Id;

            BotCommand command = await _botCommandParserService.ParseCommand(text, userChatId);
            if (command != null)
                command.UserChatId = userChatId;

            if (command is ShowTodoCommand showTodoCommand)
            {
                await _showTodoCommandProcessor.ProcessAsync(showTodoCommand);
                return;
            }

            if (command is StartCommand startCommand)
            {
                await _startCommandProcessor.ProcessAsync(startCommand);
                return;
            }

            if (command is CreateTaskCommand createTaskCommand)
            {
                await _createTaskCommandProcessor.ProcessAsync(createTaskCommand);
                return;
            }

            await _botSendMessageService.SendUnknownCommandAsync(userChatId);
        }
    }
}