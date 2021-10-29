using MongoDB.Driver;

namespace DarkDeeds.TaskServiceApp.Data
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly string _connectionString;

        public MongoDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IMongoCollection<T> GetCollection<T>(string tableName)
        {
            // TODO! password
            var connectionInfo = _connectionString.Split(';');
            var database = new MongoClient($"mongodb://{connectionInfo[0]}").GetDatabase(connectionInfo[1]);
            return database.GetCollection<T>(tableName);
        }
    }
}