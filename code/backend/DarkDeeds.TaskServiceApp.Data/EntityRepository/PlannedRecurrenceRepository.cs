using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data.EntityRepository;

namespace DarkDeeds.TaskServiceApp.Data.EntityRepository
{
    public class PlannedRecurrenceRepository : Repository<PlannedRecurrenceEntity>, IPlannedRecurrenceRepository
    {
        public PlannedRecurrenceRepository() : base("plannedRecurrences")
        {
        }

        static PlannedRecurrenceRepository()
        {
            RegisterDefaultMap<PlannedRecurrenceEntity>();
            RegisterDefaultMap<RecurrenceEntity>();
        }

        public Task SaveRecurrences(PlannedRecurrenceEntity entity) => UpdatePropertiesAsync(entity, x => x.Recurrences);
    }
}