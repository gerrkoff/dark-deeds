using DarkDeeds.TaskServiceApp.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.TaskServiceApp.Data.Context
{
    public class DarkDeedsContext : DbContext
    {
        public DarkDeedsContext(DbContextOptions<DarkDeedsContext> options) : base(options)
        {
        }
        
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<PlannedRecurrenceEntity> PlannedRecurrences { get; set; }
        public DbSet<RecurrenceEntity> Recurrences { get; set; }
    }
}