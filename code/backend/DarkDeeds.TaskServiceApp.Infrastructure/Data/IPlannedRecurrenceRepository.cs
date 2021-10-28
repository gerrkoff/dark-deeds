using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;

namespace DarkDeeds.TaskServiceApp.Infrastructure.Data
{
    public interface IPlannedRecurrenceRepository : IRepository<PlannedRecurrenceEntity>

    {
    // IQueryable<PlannedRecurrenceEntity> GetAll();
    // Task<PlannedRecurrenceEntity> GetByIdAsync(int id);
    // Task SaveAsync(PlannedRecurrenceEntity entity);
    // Task DeleteAsync(int id);
    }
}