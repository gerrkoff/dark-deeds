using DarkDeeds.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.Data.Context
{
    public class DarkDeedsContext : DbContext
    {
        public DarkDeedsContext(DbContextOptions<DarkDeedsContext> options) : base(options)
        {
        }
        
        public DbSet<SettingsEntity> Settings { get; set; }
    }
}