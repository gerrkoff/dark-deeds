using System;

namespace DarkDeeds.BotIntegration.Objects.Commands
{
    public class ShowTodoCommand : BotCommand
    {
        public DateTime From { get; }
        public DateTime To { get; }

        private readonly string _args;

        public ShowTodoCommand(string args, int timeAdjustment)
        {
            _args = args;
            if (string.IsNullOrWhiteSpace(args))
            {
                From = DateTime.UtcNow.AddMinutes(timeAdjustment).Date;
            }
            else if (args.Length == 4)
            {
                int year = DateTime.UtcNow.AddMinutes(timeAdjustment).Year;
                int month = int.Parse(args.Substring(0, 2));
                int day = int.Parse(args.Substring(2, 2));
                From = new DateTime(year, month, day); 
            }
            else if (args.Length == 8)
            {
                int year = int.Parse(args.Substring(0, 4));
                int month = int.Parse(args.Substring(4, 2));
                int day = int.Parse(args.Substring(6, 2));
                From = new DateTime(year, month, day);
            }

            To = From.AddDays(1);
        }
        
        public override string ToString()
        {
            return $"{nameof(ShowTodoCommand)} args='{_args}' {base.ToString()}";
        }
    }
}