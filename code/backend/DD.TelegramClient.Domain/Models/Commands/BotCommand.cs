namespace DD.TelegramClient.Domain.Models.Commands;

public abstract class BotCommand
{
    public int UserChatId { get; set; }

    public override string ToString()
    {
        return $"ChatId={UserChatId}";
    }
}
