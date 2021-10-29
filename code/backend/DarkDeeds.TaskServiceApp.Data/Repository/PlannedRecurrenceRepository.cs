using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        public async Task<PlannedRecurrenceEntity> GetByIdAsync(string id)
        { 
            return await (await _collection.FindAsync(x => x.Uid == id)).SingleOrDefaultAsync();
        }

        public Task<IList<PlannedRecurrenceEntity>> GetBySpecAsync(ISpecification<PlannedRecurrenceEntity> spec)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<PlannedRecurrenceEntity>> GetListAsync()
        {
            return await (await _collection.FindAsync(x => true)).ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<PlannedRecurrenceEntity, bool>> predicate)
        {
            return await (await _collection.FindAsync(predicate)).AnyAsync();
        }

        public async Task SaveAsync(PlannedRecurrenceEntity entity)
        {
            var exists = await _collection.CountDocumentsAsync(x => x.Uid == entity.Uid) > 0;

            if (!exists)
            {
                await _collection.InsertOneAsync(entity);
                return;
            }

            var q = await _collection.ReplaceOneAsync(x => x.Uid == entity.Uid, entity);
        }

        public async Task SaveRecurrences(PlannedRecurrenceEntity entity)
        {
            var update = Builders<PlannedRecurrenceEntity>.Update
                .Set(x => x.Recurrences, entity.Recurrences);
            await _collection.UpdateOneAsync(x => x.Uid == entity.Uid, update);
        }

        

        public Task DeleteAsync(string id) => _collection.DeleteOneAsync(x => x.Uid == id);
    }
}