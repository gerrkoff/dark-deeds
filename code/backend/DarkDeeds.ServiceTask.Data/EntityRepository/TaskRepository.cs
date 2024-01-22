using DD.TaskService.Domain.Entities;
using DD.TaskService.Domain.Infrastructure.EntityRepository;

namespace DarkDeeds.ServiceTask.Data.EntityRepository;

public class TaskRepository : Repository<TaskEntity>, ITaskRepository
{
    public TaskRepository(IMongoDbContext dbContext) : base(dbContext, "tasks")
    {
    }

    static TaskRepository()
    {
        RegisterDefaultMap<TaskEntity>();
    }
}
