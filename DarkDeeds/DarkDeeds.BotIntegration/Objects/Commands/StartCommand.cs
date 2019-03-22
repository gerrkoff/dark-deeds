using System;

namespace DarkDeeds.BotIntegration.Objects.Commands
{
    public class StartCommand : BotCommand
    {
        public string UserChatKey { get; }

        public StartCommand(string userChatKey)
        {
            UserChatKey = userChatKey;
        }
    }
}