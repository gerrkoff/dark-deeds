using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DarkDeeds.TaskServiceApp.Data.Repository
{
    // TODO: cleanup
    public class PlannedRecurrenceRepository : Repository<PlannedRecurrenceEntity>, IPlannedRecurrenceRepository
    {
        private readonly IMongoCollection<PlannedRecurrenceEntity> _collection;
        
        public PlannedRecurrenceRepository()
        {   
            var database = new MongoClient("mongodb://192.168.0.199:27017").GetDatabase("dark-deeds-task-service");
            _collection = database.GetCollection<PlannedRecurrenceEntity>("plannedRecurrences");
        }

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

        protected override IMongoCollection<PlannedRecurrenceEntity> Collection => _collection;

        public override async Task SaveRecurrences(PlannedRecurrenceEntity entity)
        {
            var update = Builders<PlannedRecurrenceEntity>.Update
                .Set(x => x.Recurrences, entity.Recurrences);
            await _collection.UpdateOneAsync(x => x.Uid == entity.Uid, update);
        }
    }
}