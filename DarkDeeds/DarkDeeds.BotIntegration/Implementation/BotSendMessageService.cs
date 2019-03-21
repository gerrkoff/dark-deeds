using System.Diagnostics;
using DarkDeeds.BotIntegration.Interface;

namespace DarkDeeds.BotIntegration.Implementation
{
    public class BotSendMessageService : IBotSendMessageService
    {
        public void SendUnknownCommand()
        {
            Debug.WriteLine("");
            Debug.WriteLine("_________________________________Unknown command");
            Debug.WriteLine("");
        }

        public void SendText(string text)
        {
            Debug.WriteLine("");
            Debug.WriteLine($"_________________________________{text}");
            Debug.WriteLine("");
        }
    }
}