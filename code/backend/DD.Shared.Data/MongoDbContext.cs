using MongoDB.Driver;

namespace DD.Shared.Data;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string tableName);
}

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString)
    {
        // TODO: This is a hack, fix it
        var db = connectionString.Split('/').Last().Split('?').First();
        _database = new MongoClient(connectionString).GetDatabase(db);
    }

    public IMongoCollection<T> GetCollection<T>(string tableName)
    {
        return _database.GetCollection<T>(tableName);
    }
}
