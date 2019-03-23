using System;

namespace DarkDeeds.BotIntegration.Objects.Commands
{
    public class ShowTodoCommand : BotCommand
    {
        public DateTime From { get; }
        public DateTime To { get; }

        public ShowTodoCommand(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                From = DateTime.Today.AddHours(-5);
                To = DateTime.Today.AddHours(29);
            }
        }
    }
}