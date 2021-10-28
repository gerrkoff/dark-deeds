using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;

namespace DarkDeeds.TaskServiceApp.Infrastructure.Data
{
    public interface ITaskRepository
    {
        IQueryable<TaskEntity> GetAll();
        Task<IList<TaskEntity>> GetBySpecAsync(ISpecification<TaskEntity> spec);
        Task SaveAsync(TaskEntity entity);
        Task DeleteAsync(string id);
        
        // TODO: save props
        // TODO: hard delete
    }
}