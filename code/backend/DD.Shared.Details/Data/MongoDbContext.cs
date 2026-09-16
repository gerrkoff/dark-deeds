using MongoDB.Driver;

namespace DD.Shared.Details.Data;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string tableName);
}

public interface IMigratorMongoDbContext : IMongoDbContext
{
    IMongoClient Client { get; }
}

public class MongoDbContext : IMigratorMongoDbContext
{
    public MongoDbContext(string connectionString)
    {
        Client = new MongoClient(connectionString);

        var databaseName = new MongoUrl(connectionString).DatabaseName
            ?? throw new ArgumentException(
                "The MongoDB connection string must contain a database name.",
                nameof(connectionString));
        Database = Client.GetDatabase(databaseName);
    }

    public IMongoClient Client { get; }

    private IMongoDatabase Database { get; }

    public IMongoCollection<T> GetCollection<T>(string tableName)
    {
        return Database.GetCollection<T>(tableName);
    }
}
