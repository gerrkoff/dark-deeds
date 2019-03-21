namespace DarkDeeds.BotIntegration.Interface
{
    public interface IBotSendMessageService
    {
        void SendUnknownCommand();
        void SendText(string text);
    }
}