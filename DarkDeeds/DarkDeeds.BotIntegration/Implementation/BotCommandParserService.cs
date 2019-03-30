using System.Threading.Tasks;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.BotIntegration.Implementation
{
    public class BotCommandParserService : IBotCommandParserService
    {
        const string TodoCommand = "/todo";
        const string StartCommand = "/start";

        private readonly ITaskParserService _taskParserService;
        private readonly ITelegramService _telegramService;

        public BotCommandParserService(ITaskParserService taskParserService, ITelegramService telegramService)
        {
            _taskParserService = taskParserService;
            _telegramService = telegramService;
        }

        public async Task<BotCommand> ParseCommand(string command, int chatId)
        {
            string args;
            // TODO: use user time adjustment
            if (CheckAndTrimCommand(TodoCommand, command, out args))
                return new ShowTodoCommand(args);
            
            if (CheckAndTrimCommand(StartCommand, command, out args))
                return new StartCommand(args);

            if (!command.StartsWith("/"))
            {
                int timeAdjustment = await _telegramService.GetUserTimeAdjustment(chatId);
                return new CreateTaskCommand(_taskParserService.ParseTask(command, timeAdjustment));
            }

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