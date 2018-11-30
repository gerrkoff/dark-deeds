using DarkDeeds.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.Data.Context
{
    public class DarkDeedsContext : DbContext
    {
        public DarkDeedsContext(DbContextOptions<DarkDeedsContext> options) : base(options)
        {
        }
        
        public DbSet<TaskEntity> Tasks { get; set; }
    }
}