using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;

namespace DarkDeeds.TaskServiceApp.Infrastructure.Data
{
    public interface IPlannedRecurrenceRepository : IRepository<PlannedRecurrenceEntity>
    {
        // TODO!
        Task SaveRecurrences(PlannedRecurrenceEntity entity);
    }
}