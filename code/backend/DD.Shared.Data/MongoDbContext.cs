using MongoDB.Driver;

namespace DD.Shared.Data;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string tableName);
}

public interface IMigratorMongoDbContext : IMongoDbContext
{
    public IMongoClient Client { get; }
}

public class MongoDbContext : IMigratorMongoDbContext
{
    public MongoDbContext(string connectionString)
    {
        Client = new MongoClient(connectionString);

        // TODO: This is a hack, fix it
        var db = connectionString.Split('/').Last().Split('?').First();
        Database = Client.GetDatabase(db);
    }

    public IMongoClient Client { get; }

    private IMongoDatabase Database { get; }

    public IMongoCollection<T> GetCollection<T>(string tableName)
    {
        return Database.GetCollection<T>(tableName);
    }
}
