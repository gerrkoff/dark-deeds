using DD.TaskService.Domain.Entities;
using DD.TaskService.Domain.Infrastructure.EntityRepository;

namespace DarkDeeds.ServiceTask.Data.EntityRepository;

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
