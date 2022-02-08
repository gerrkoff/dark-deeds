using MongoDB.Driver;

namespace DarkDeeds.ServiceTask.Data
{
    interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string tableName);
    }
}