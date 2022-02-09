using DarkDeeds.ServiceTask.Entities.Models;
using DarkDeeds.ServiceTask.Infrastructure.Data.EntityRepository;

namespace DarkDeeds.ServiceTask.Data.EntityRepository
{
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
}