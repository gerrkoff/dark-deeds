using System;

namespace DarkDeeds.BotIntegration.Objects.Commands
{
    public class ShowTodoCommand : BotCommand
    {
        public DateTime From { get; }
        public DateTime To { get; }

        public ShowTodoCommand(string args, int timeAdjustment)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                From = DateTime.Today;
                To = DateTime.Today.AddDays(1);
            }

            From = From.AddMinutes(timeAdjustment);
            To = To.AddMinutes(timeAdjustment);
        }
    }
}