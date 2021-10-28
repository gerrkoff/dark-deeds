using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.TaskServiceApp.Data.Repository
{
    public class TaskRepository : Repository<TaskEntity>, ITaskRepository
    {
        public TaskRepository(DbContext context) : base(context)
        {
        }
    }
}