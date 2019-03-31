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
            }
            else if (args.Length == 4)
            {
                int month = int.Parse(args.Substring(0, 2));
                int day = int.Parse(args.Substring(2, 2));
                From = new DateTime(DateTime.UtcNow.Year, month, day); 
            }
            else if (args.Length == 8)
            {
                int year = int.Parse(args.Substring(0, 4));
                int month = int.Parse(args.Substring(4, 2));
                int day = int.Parse(args.Substring(6, 2));
                From = new DateTime(year, month, day);
            }

            From = From.AddMinutes(timeAdjustment);
            To = From.AddDays(1);
        }
    }
}