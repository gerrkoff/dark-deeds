using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data.EntityRepository;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DarkDeeds.TaskServiceApp.Data.EntityRepository
{
    public class PlannedRecurrenceRepository : Repository<PlannedRecurrenceEntity>, IPlannedRecurrenceRepository
    {
        public PlannedRecurrenceRepository()
        {
            Collection = Database.GetCollection<PlannedRecurrenceEntity>("plannedRecurrences");
        }

        protected override IMongoCollection<PlannedRecurrenceEntity> Collection { get; }

        static PlannedRecurrenceRepository()
        {
            BsonClassMap.RegisterClassMap<PlannedRecurrenceEntity>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
            
            BsonClassMap.RegisterClassMap<RecurrenceEntity>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
        }

        public Task SaveRecurrences(PlannedRecurrenceEntity entity) => UpdatePropertiesAsync(entity, x => x.Recurrences);
    }
}