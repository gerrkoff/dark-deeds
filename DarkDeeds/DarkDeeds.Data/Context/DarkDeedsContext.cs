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
        
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<SettingsEntity> Settings { get; set; }
        public DbSet<PlannedRecurrenceEntity> PlannedRecurrences { get; set; }
        public DbSet<RecurrenceEntity> Recurrences { get; set; }
    }
}