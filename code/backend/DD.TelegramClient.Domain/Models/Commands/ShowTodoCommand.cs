using System.Globalization;

namespace DD.TelegramClient.Domain.Models.Commands;

public class ShowTodoCommand : BotCommand
{
    private readonly string _args;

    public ShowTodoCommand(string args, DateTime now, int timeAdjustment)
    {
        _args = args;
        if (string.IsNullOrWhiteSpace(args))
        {
            From = now.AddMinutes(timeAdjustment).Date;
        }
        else if (args.Length == 4)
        {
            var year = now.AddMinutes(timeAdjustment).Year;
            var month = int.Parse(args[..2], CultureInfo.InvariantCulture);
            var day = int.Parse(args[2..4], CultureInfo.InvariantCulture);
            From = new DateTime(year, month, day);
        }
        else if (args.Length == 8)
        {
            var year = int.Parse(args[..4], CultureInfo.InvariantCulture);
            var month = int.Parse(args[4..6], CultureInfo.InvariantCulture);
            var day = int.Parse(args[6..8], CultureInfo.InvariantCulture);
            From = new DateTime(year, month, day);
        }

        To = From.AddDays(1);
    }

    public DateTime From { get; }

    public DateTime To { get; }

    public override string ToString()
    {
        return $"{nameof(ShowTodoCommand)} args='{_args}' {base.ToString()}";
    }
}
