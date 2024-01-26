using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;

namespace DD.ServiceTask.Details.Data.EntityRepository;

class TaskRepository(IMongoDbContext dbContext) : Repository<TaskEntity>(dbContext, "tasks"), ITaskRepository
{
    static TaskRepository()
    {
        RegisterDefaultMap<TaskEntity>();
    }
}
