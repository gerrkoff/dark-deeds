using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data.EntityRepository;

namespace DarkDeeds.TaskServiceApp.Data.EntityRepository
{
    public class PlannedRecurrenceRepository : Repository<PlannedRecurrenceEntity>, IPlannedRecurrenceRepository
    {
        public PlannedRecurrenceRepository(IMongoDbContext dbContext) : base(dbContext, "plannedRecurrences")
        {
        }

        static PlannedRecurrenceRepository()
        {
            RegisterDefaultMap<PlannedRecurrenceEntity>();
            RegisterDefaultMap<RecurrenceEntity>();
        }
    }
}