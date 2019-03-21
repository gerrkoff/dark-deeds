using DarkDeeds.BotIntegration.Dto;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Objects.Commands;

namespace DarkDeeds.BotIntegration.Implementation
{
    public class BotProcessMessageService : IBotProcessMessageService
    {
        private readonly IBotSendMessageService _botSendMessageService;

        public BotProcessMessageService(IBotSendMessageService botSendMessageService)
        {
            _botSendMessageService = botSendMessageService;
        }

        public void ProcessMessage(UpdateDto update)
        {
            string text = update.Message.Text.Trim();
            
            const string todo = "/todo";

            if (CheckAndTrimCommand(todo, text, out var args))
            {
                ProcessShowTodoCommand(new ShowTodoCommand(args));
                return;
            }

            if (!text.StartsWith("/"))
            {
                ProcessCreateTaskCommand(new CreateTaskCommand(text));
                return;
            }
            
            _botSendMessageService.SendUnknownCommand();
        }

        public bool CheckAndTrimCommand(string command, string text, out string args)
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

        private void ProcessShowTodoCommand(ShowTodoCommand cmd)
        {
            _botSendMessageService.SendText($"Show todo {cmd.Day}");
        }
        
        private void ProcessCreateTaskCommand(CreateTaskCommand cmd)
        {
            _botSendMessageService.SendText($"Create task: {cmd.Task}");
        }
    }
}