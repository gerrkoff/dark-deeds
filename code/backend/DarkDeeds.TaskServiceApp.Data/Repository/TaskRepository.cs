using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DarkDeeds.TaskServiceApp.Data.Repository
{
    // TODO: cleanup
    public class TaskRepository : ITaskRepository
    {
        private readonly IMongoCollection<TaskEntity> _collection;
        
        public TaskRepository()
        {   
            var database = new MongoClient("mongodb://192.168.0.199:27017").GetDatabase("dark-deeds-task-service");
            _collection = database.GetCollection<TaskEntity>("tasks");
        }

        static TaskRepository()
        {
            BsonClassMap.RegisterClassMap<TaskEntity>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
        }

        public IQueryable<TaskEntity> GetAll() =>
            _collection.AsQueryable();
        
        public async Task<IList<TaskEntity>> GetBySpecAsync(ISpecification<TaskEntity> spec)
        {
            var r = spec.Apply(_collection.AsQueryable()).ToList();
            return r;
        }

        public async Task<TaskEntity> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            } 
            return await (await _collection.FindAsync(x => x.Uid == id)).SingleOrDefaultAsync();
        }

        public async Task SaveAsync(TaskEntity entity)
        {
            var exists = await _collection.CountDocumentsAsync(x => x.Uid == entity.Uid) > 0;

            if (!exists)
            {
                await _collection.InsertOneAsync(entity);
                return;
            }

            var q = await _collection.ReplaceOneAsync(x => x.Uid == entity.Uid, entity);

            // var update = Builders<TaskEntity>.Update
            //     .Set(x => x.Title, entity.Title)
            //     .Set(x => x.Order, entity.Order)
            //     .Set(x => x.Date, entity.Date)
            //     .Set(x => x.Type, entity.Type)
            //     .Set(x => x.Time, entity.Time)
            //     .Set(x => x.IsCompleted, entity.IsCompleted)
            //     .Set(x => x.IsProbable, entity.IsProbable)
            //     .Set(x => x.Version, entity.Version)
            //     .Set(x => x.Uid, entity.Uid)
            //     .Set(x => x.UserId, entity.UserId);
            // await _collection.UpdateOneAsync(x => x.Uid == entity.Uid, update);
        }

        

        public Task DeleteAsync(string id) => _collection.DeleteOneAsync(x => x.Uid == id);
    }
}