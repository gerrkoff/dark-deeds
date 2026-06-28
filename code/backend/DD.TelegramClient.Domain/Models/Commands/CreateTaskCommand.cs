namespace DD.TelegramClient.Domain.Models.Commands;

public class CreateTaskCommand(string text) : BotCommand
{
    public string Text { get; } = text;

    public override string ToString()
    {
        return $"{nameof(CreateTaskCommand)} {base.ToString()}";
    }
}
