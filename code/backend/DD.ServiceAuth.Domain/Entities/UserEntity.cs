using Microsoft.AspNetCore.Identity;

namespace DD.ServiceAuth.Domain.Entities;

public class UserEntity : IdentityUser
{
    public string DisplayName { get; set; }
}
