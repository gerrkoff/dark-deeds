using Microsoft.AspNetCore.Identity;

namespace DarkDeeds.TelegramClient.Entities
{
    public class UserEntity : IdentityUser
    {
        public string TelegramChatKey { get; set; }
        public int TelegramChatId { get; set; }
        public int TelegramTimeAdjustment { get; set; }
    }
}