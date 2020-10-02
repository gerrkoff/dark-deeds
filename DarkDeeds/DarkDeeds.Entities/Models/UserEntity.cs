using Microsoft.AspNetCore.Identity;

namespace DarkDeeds.Entities.Models
{
    public class UserEntity : IdentityUser
    {
        public string DisplayName { get; set; }
        public string TelegramChatKey { get; set; }
        public int TelegramChatId { get; set; }
        public int TelegramTimeAdjustment { get; set; }
    }
}