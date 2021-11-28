using DarkDeeds.MongoMigrator.PostgreDal.Models;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.MongoMigrator.PostgreDal.Context
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