using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DarkDeeds.TaskServiceApp.Data.Repository
{
    // TODO: cleanup
    public class PlannedRecurrenceRepository : IPlannedRecurrenceRepository
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

        public IQueryable<PlannedRecurrenceEntity> GetAll() =>
            _collection.AsQueryable();

        public async Task<PlannedRecurrenceEntity> GetByIdAsync(string id)
        { 
            return await (await _collection.FindAsync(x => x.Uid == id)).SingleOrDefaultAsync();
        }

        public async Task SaveAsync(PlannedRecurrenceEntity entity)
        {
            var exists = await _collection.CountDocumentsAsync(x => x.Id == entity.Id) > 0;

            if (!exists)
            {
                await _collection.InsertOneAsync(entity);
                return;
            }

            var q = await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
        }

        public async Task SaveRecurrences(PlannedRecurrenceEntity entity)
        {
            var update = Builders<PlannedRecurrenceEntity>.Update
                .Set(x => x.Recurrences, entity.Recurrences);
            await _collection.UpdateOneAsync(x => x.Id == entity.Id, update);
        }

        

        public Task DeleteAsync(string id) => _collection.DeleteOneAsync(x => x.Uid == id);
    }
}