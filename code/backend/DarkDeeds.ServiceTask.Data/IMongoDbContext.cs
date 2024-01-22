using MongoDB.Driver;

namespace DarkDeeds.ServiceTask.Data;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string tableName);
}
