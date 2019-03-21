using System;

namespace DarkDeeds.BotIntegration.Objects.Commands
{
    public class ShowTodoCommand : BotCommand
    {
        public DateTime Day { get; }

        public ShowTodoCommand(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
                Day = DateTime.Today;
        }
    }
}