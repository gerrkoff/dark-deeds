using DarkDeeds.WebClientBffApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.WebClientBffApp.Data.Context
{
    public class DarkDeedsWebClientBffContext : DbContext
    {
        public DarkDeedsWebClientBffContext(DbContextOptions<DarkDeedsWebClientBffContext> options) : base(options)
        {
        }
        
        public DbSet<SettingsEntity> Settings { get; set; }
    }
}