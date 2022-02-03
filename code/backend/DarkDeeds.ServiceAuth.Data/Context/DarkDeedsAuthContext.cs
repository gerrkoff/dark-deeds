using DarkDeeds.ServiceAuth.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.ServiceAuth.Data.Context
{
    public class DarkDeedsAuthContext : IdentityDbContext<UserEntity>
    {
        public DarkDeedsAuthContext(DbContextOptions<DarkDeedsAuthContext> options) : base(options)
        {
        }
    }
}