using MongoDB.Driver;

namespace DD.ServiceTask.Details.Data;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string tableName);
}
