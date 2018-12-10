using Microsoft.AspNetCore.Identity;

namespace DarkDeeds.Data.Entity
{
    public class UserEntity : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}