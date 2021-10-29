using System;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DarkDeeds.TaskServiceApp.Data.Repository
{
    // TODO: cleanup
    public class TaskRepository : Repository<TaskEntity>, ITaskRepository
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

        protected override IMongoCollection<TaskEntity> Collection => _collection;

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
        
        // TODO! fix
        // var update = Builders<PlannedRecurrenceEntity>.Update
        //         .Set(x => x.IsDeleted, true);   
        //     
        //     return _collection.UpdateOneAsync(x => x.Id == id, update);

        public override Task SaveRecurrences(TaskEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}