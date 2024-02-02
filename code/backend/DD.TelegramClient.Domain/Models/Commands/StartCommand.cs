namespace DD.TelegramClient.Domain.Models.Commands;

public class StartCommand(string userChatKey) : BotCommand
{
    public string UserChatKey { get; } = userChatKey;

    public override string ToString()
    {
        return $"{nameof(ShowTodoCommand)} ChatKey='{UserChatKey}' {base.ToString()}";
    }
}
