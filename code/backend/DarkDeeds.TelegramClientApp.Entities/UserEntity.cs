using Microsoft.AspNetCore.Identity;

namespace DarkDeeds.TelegramClientApp.Entities
{
    public class UserEntity : IdentityUser
    {
        public string TelegramChatKey { get; set; }
        public int TelegramChatId { get; set; }
        public int TelegramTimeAdjustment { get; set; }
    }
}