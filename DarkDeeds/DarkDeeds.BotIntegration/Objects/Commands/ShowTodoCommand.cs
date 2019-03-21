using System;

namespace DarkDeeds.BotIntegration.Objects.Commands
{
    public class ShowTodoCommand
    {
        public DateTime Day { get; private set; }

        public ShowTodoCommand(string args)
        {
            Day = DateTime.Today;
        }
    }
}