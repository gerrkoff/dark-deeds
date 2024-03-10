using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;
using DD.Shared.Data;

namespace DD.ServiceTask.Details.Data;

internal sealed class TaskRepository(IMongoDbContext dbContext) : Repository<TaskEntity>(dbContext, "tasks"), ITaskRepository
{
    static TaskRepository()
    {
        RegisterDefaultMap<TaskEntity>();
    }
}
