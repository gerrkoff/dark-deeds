using Microsoft.AspNetCore.Identity;

namespace DarkDeeds.Data.Entity
{
    public class UserEntity : IdentityUser
    {
        public string DisplayName { get; set; }
        public string TelegramChatKey { get; set; }
        public int TelegramChatId { get; set; }
    }
}