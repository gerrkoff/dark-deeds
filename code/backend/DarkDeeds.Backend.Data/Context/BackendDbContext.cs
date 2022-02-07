using DarkDeeds.Backend.Entities.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.Backend.Data.Context
{
    public class BackendDbContext : IdentityDbContext<UserEntity>
    {
        public BackendDbContext(DbContextOptions<BackendDbContext> options) : base(options)
        {
        }
        
        public DbSet<SettingsEntity> Settings { get; set; }
    }
}