using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;

namespace DarkDeeds.TaskServiceApp.Infrastructure.Data
{
    public interface IPlannedRecurrenceRepository
    {
        IQueryable<PlannedRecurrenceEntity> GetAll();
        Task<PlannedRecurrenceEntity> GetByIdAsync(string id);
        Task SaveAsync(PlannedRecurrenceEntity entity);
        Task DeleteAsync(string id);
        Task SaveRecurrences(PlannedRecurrenceEntity entity);
    }
}