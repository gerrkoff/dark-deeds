namespace DarkDeeds.Services.Interface
{
    public interface IBotSendMessageService
    {
        void SendUnknownCommand();
        void SendText(string text);
    }
}