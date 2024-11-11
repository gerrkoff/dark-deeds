using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;
using DD.Shared.Details.Data;

namespace DD.ServiceTask.Details.Data;

public sealed class TaskRepository(IMongoDbContext dbContext) : Repository<TaskEntity>(dbContext, "tasks"), ITaskRepository
{
    static TaskRepository()
    {
        RegisterDefaultMap<TaskEntity>();
    }
}
