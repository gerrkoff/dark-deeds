using DarkDeeds.TaskServiceApp.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.TaskServiceApp.Data.Context
{
    public class DarkDeedsTaskContext : DbContext
    {
        public DarkDeedsTaskContext(DbContextOptions<DarkDeedsTaskContext> options) : base(options)
        {
        }
        
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<PlannedRecurrenceEntity> PlannedRecurrences { get; set; }
        public DbSet<RecurrenceEntity> Recurrences { get; set; }
    }
}