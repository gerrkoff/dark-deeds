using System;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Data;
using MongoDB.Driver;

namespace DarkDeeds.MongoMigrator.MongoDal
{
    public class MigratedEntitiesRepository
    {
        private readonly IMongoCollection<MigratedEntity> _collection;
        
        public MigratedEntitiesRepository(IMongoDbContext dbContext)
        {
            _collection = dbContext.GetCollection<MigratedEntity>("pgMigratedEntities");
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var builder = Builders<MigratedEntity>.IndexKeys;
            var indexModel = new CreateIndexModel<MigratedEntity>(builder.Ascending(x => x.EntityUid));
            _collection.Indexes.CreateOneAsync(indexModel);
        }
        
        public async Task<bool> ExistsAsync(string uid, MigratedEntityType type)
        {
            if (string.IsNullOrWhiteSpace(uid))
                throw new ArgumentNullException(nameof(uid));

            using var cursor = await _collection.FindAsync(x => x.EntityUid == uid && x.EntityType == type);
            return await cursor.AnyAsync();
        }
        
        public async Task InsertAsync(MigratedEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _collection.InsertOneAsync(entity);
        }
    }
}