using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DarkDeeds.TaskServiceApp.Data.Repository
{
    // TODO: cleanup
    public class TaskRepository : Repository<TaskEntity>, ITaskRepository
    {
        public TaskRepository()
        {   
            var database = new MongoClient("mongodb://192.168.0.199:27017").GetDatabase("dark-deeds-task-service");
            Collection = database.GetCollection<TaskEntity>("tasks");
        }

        static TaskRepository()
        {
            BsonClassMap.RegisterClassMap<TaskEntity>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
        }

        protected override IMongoCollection<TaskEntity> Collection { get; }
    }
}