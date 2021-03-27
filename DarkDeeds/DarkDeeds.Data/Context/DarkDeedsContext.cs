using DarkDeeds.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DarkDeeds.Data.Context
{
    public class DarkDeedsContext : IdentityDbContext<UserEntity>
    {
        public DarkDeedsContext(DbContextOptions<DarkDeedsContext> options) : base(options)
        {
        }
        
        public DbSet<SettingsEntity> Settings { get; set; }
    }
}