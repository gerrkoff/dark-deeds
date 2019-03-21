namespace DarkDeeds.BotIntegration.Interface
{
    public interface IBotSendMessageService
    {
        void SendUnknownCommand(int userChatId);
        void SendText(int userChatId, string text);
    }
}