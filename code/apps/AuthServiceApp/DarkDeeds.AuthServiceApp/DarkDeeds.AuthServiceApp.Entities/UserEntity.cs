using Microsoft.AspNetCore.Identity;

namespace DarkDeeds.AuthServiceApp.Entities
{
    public class UserEntity : IdentityUser
    {
        public string DisplayName { get; set; }
        public string TelegramChatKey { get; set; }
        public int TelegramChatId { get; set; }
        public int TelegramTimeAdjustment { get; set; }
    }
}