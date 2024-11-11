using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;
using DD.Shared.Details.Data;

namespace DD.ServiceTask.Details.Data;

public sealed class PlannedRecurrenceRepository(IMongoDbContext dbContext)
    : Repository<PlannedRecurrenceEntity>(dbContext, "plannedRecurrences"), IPlannedRecurrenceRepository
{
    static PlannedRecurrenceRepository()
    {
        RegisterDefaultMap<PlannedRecurrenceEntity>();
        RegisterDefaultMap<RecurrenceEntity>();
    }
}
