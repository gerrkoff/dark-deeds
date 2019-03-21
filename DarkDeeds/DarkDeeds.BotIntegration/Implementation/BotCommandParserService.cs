using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.BotIntegration.Implementation
{
    public class BotCommandParserService : IBotCommandParserService
    {
        const string Todo = "/todo";

        private readonly ITaskParserService _taskParserService;

        public BotCommandParserService(ITaskParserService taskParserService)
        {
            _taskParserService = taskParserService;
        }

        public BotCommand ParseCommand(string command)
        {
            if (CheckAndTrimCommand(Todo, command, out var args))
                return new ShowTodoCommand(args);

            if (!command.StartsWith("/"))
                return new CreateTaskCommand(_taskParserService.ParseTask(command));

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