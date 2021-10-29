using MongoDB.Driver;

namespace DarkDeeds.TaskServiceApp.Data
{
    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string tableName);
    }
}