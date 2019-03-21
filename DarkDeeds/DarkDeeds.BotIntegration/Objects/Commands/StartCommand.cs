using System;

namespace DarkDeeds.BotIntegration.Objects.Commands
{
    public class StartCommand : BotCommand
    {
        public Guid UserChatKey { get; }

        public StartCommand(string userChatKey)
        {
            UserChatKey = Guid.Parse(userChatKey);
        }
    }
}