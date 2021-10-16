namespace DarkDeeds.TelegramClientApp.Services.Models.Commands
{
    public class StartCommand : BotCommand
    {
        public string UserChatKey { get; }

        public StartCommand(string userChatKey)
        {
            UserChatKey = userChatKey;
        }
        
        public override string ToString()
        {
            return $"{nameof(ShowTodoCommand)} ChatKey='{UserChatKey}' {base.ToString()}";
        }
    }
}