using System;
using System.Threading.Tasks;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.TelegramClientApp.Services.Interface;
using DarkDeeds.TelegramClientApp.Services.Models.Commands;

namespace DarkDeeds.TelegramClientApp.Services.Implementation
{
    public class BotCommandParserService : IBotCommandParserService
    {
        const string TodoCommand = "/todo";
        const string StartCommand = "/start";

        private readonly ITelegramService _telegramService;
        private readonly IDateService _dateService;
        private readonly ITaskServiceApp _taskServiceApp;

        public BotCommandParserService(ITelegramService telegramService, IDateService dateService, ITaskServiceApp taskServiceApp)
        {
            _telegramService = telegramService;
            _dateService = dateService;
            _taskServiceApp = taskServiceApp;
        }

        public async Task<BotCommand> ParseCommand(string command, int chatId)
        {
            if (CheckAndTrimCommand(StartCommand, command, out var args))
                return new StartCommand(args);
            
            if (CheckAndTrimCommand(TodoCommand, command, out args))
            {
                int timeAdjustment = await _telegramService.GetUserTimeAdjustment(chatId);
                DateTime now = _dateService.Now;
                return new ShowTodoCommand(args, now, timeAdjustment);
            }

            if (!command.StartsWith("/"))
                return new CreateTaskCommand(await _taskServiceApp.ParseTask(command));

            return null;
        }
        
        private bool CheckAndTrimCommand(string command, string text, out string args)
        {
            args = string.Empty;

            if (string.Equals(text, command))
                return true;

            if (text.StartsWith(command + " "))
            {
                args = text.Substring(command.Length + 1).Trim();
                return true;
            }

            return false;
        }
    }
}