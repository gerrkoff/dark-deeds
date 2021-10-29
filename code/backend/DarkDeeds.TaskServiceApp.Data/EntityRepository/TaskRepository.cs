using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data.EntityRepository;

namespace DarkDeeds.TaskServiceApp.Data.EntityRepository
{
    public class TaskRepository : Repository<TaskEntity>, ITaskRepository
    {
        public TaskRepository() : base("tasks")
        {
        }

        static TaskRepository()
        {
            RegisterDefaultMap<TaskEntity>();
        }
    }
}