using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DarkDeeds.TaskServiceApp.Data.Repository
{
    public class TaskRepository : Repository<TaskEntity>, ITaskRepository
    {
        public TaskRepository()
        {
            Collection = Database.GetCollection<TaskEntity>("tasks");
        }

        protected override IMongoCollection<TaskEntity> Collection { get; }

        static TaskRepository()
        {
            BsonClassMap.RegisterClassMap<TaskEntity>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
        }
    }
}