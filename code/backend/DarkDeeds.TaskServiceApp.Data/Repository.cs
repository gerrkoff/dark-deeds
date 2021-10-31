using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models.Abstractions;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DarkDeeds.TaskServiceApp.Data
{
    // TODO! using cursor
    public abstract class Repository<T> : IRepository<T>
        where T: Entity
    {
        private readonly IMongoCollection<T> _collection;

        protected Repository(IMongoDbContext dbContext, string tableName)
        {
            _collection = dbContext.GetCollection<T>(tableName);
        }

        protected static void RegisterDefaultMap<TEntity>()
        {
            BsonClassMap.RegisterClassMap<TEntity>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
        }

        public async Task<T> GetByIdAsync(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
                throw new ArgumentNullException(nameof(uid));

            var cursor = await _collection.FindAsync(x => x.Uid == uid);
            return await cursor.SingleOrDefaultAsync();
        }

        public Task<IList<T>> GetBySpecAsync(ISpecification<T> spec)
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));

            // TODO! refactor, evaluator
            return Task.FromResult(spec.Apply(_collection.AsQueryable()).ToList() as IList<T>);
        }

        public async Task<IList<T>> GetListAsync()
        {
            var cursor = await _collection.FindAsync(x => true);
            return await cursor.ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var cursor = await _collection.FindAsync(predicate);
            return await cursor.AnyAsync();
        }

        public async Task UpsertAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            await _collection.ReplaceOneAsync(x => x.Uid == entity.Uid, entity, new ReplaceOptions
            {
                IsUpsert = true
            });
        }

        public async Task<(bool, T)> TryUpdateVersionAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var version = entity.Version;
            entity.Version++;

            var filter = Builders<T>.Filter
                .Where(x => x.Uid == entity.Uid && x.Version == version);

            var result = await _collection.ReplaceOneAsync(filter, entity, new ReplaceOptions
            {
                IsUpsert = false
            });

            if (result.MatchedCount == 1)
                return (true, null);

            entity.Version--;
            var current = await GetByIdAsync(entity.Uid);
            return (false, current);
        }

        public async Task<(bool, T)> TryUpdatePropsAsync(T entity, params Expression<Func<T, object>>[] properties)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (string.IsNullOrWhiteSpace(entity.Uid))
                throw new ArgumentNullException(nameof(entity.Uid));
            
            var version = entity.Version;
            entity.Version++;
            
            var filter = Builders<T>.Filter
                .Where(x => x.Uid == entity.Uid && x.Version == version);
            var update = Builders<T>.Update.Combine();
            
            foreach (var property in properties)
            {
                var value = property.Compile()(entity);
                update = update.Set(property, value);
            }

            var result = await _collection.UpdateOneAsync(filter, update, new UpdateOptions
            {
                IsUpsert = false
            });

            if (result.MatchedCount == 1)
                return (true, null);

            entity.Version--;
            var current = await GetByIdAsync(entity.Uid);
            return (false, current);
        }

        public Task UpdatePropertiesAsync(T entity, params Expression<Func<T, object>>[] properties)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (string.IsNullOrWhiteSpace(entity.Uid))
                throw new ArgumentNullException(nameof(entity.Uid));
            
            var update = Builders<T>.Update.Combine();

            foreach (var property in properties)
            {
                var value = property.Compile()(entity);
                update = update.Set(property, value);
            }

            return _collection.UpdateOneAsync(x => x.Uid == entity.Uid, update);
        }

        public Task DeleteAsync(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
                throw new ArgumentNullException(nameof(uid));

            var update = Builders<T>.Update.Set(x => x.IsDeleted, true);
            return _collection.UpdateOneAsync(x => x.Uid == uid, update, new UpdateOptions
            {
                IsUpsert = false
            });
        }

        public Task DeleteHardAsync(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
                throw new ArgumentNullException(nameof(uid));

            return _collection.DeleteOneAsync(x => x.Uid == uid);
        }
    }
}