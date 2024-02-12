using MongoDB.Driver;

namespace DD.ServiceTask.Details.Data;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString)
    {
        var connectionInfo = connectionString.Split(';');
        _database = new MongoClient(connectionInfo[0]).GetDatabase(connectionInfo[1]);
    }

    public IMongoCollection<T> GetCollection<T>(string tableName)
    {
        return _database.GetCollection<T>(tableName);
    }
}