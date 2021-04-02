using Microsoft.AspNetCore.Identity;

namespace DarkDeeds.AuthServiceApp.Entities
{
    public class UserEntity : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}