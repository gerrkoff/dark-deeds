using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;

namespace DD.ServiceTask.Details.Data.EntityRepository;

class PlannedRecurrenceRepository(IMongoDbContext dbContext)
    : Repository<PlannedRecurrenceEntity>(dbContext, "plannedRecurrences"), IPlannedRecurrenceRepository
{
    static PlannedRecurrenceRepository()
    {
        RegisterDefaultMap<PlannedRecurrenceEntity>();
        RegisterDefaultMap<RecurrenceEntity>();
    }
}
