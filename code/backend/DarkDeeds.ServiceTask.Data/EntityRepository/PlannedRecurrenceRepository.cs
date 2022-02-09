using DarkDeeds.ServiceTask.Entities.Models;
using DarkDeeds.ServiceTask.Infrastructure.Data.EntityRepository;

namespace DarkDeeds.ServiceTask.Data.EntityRepository
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