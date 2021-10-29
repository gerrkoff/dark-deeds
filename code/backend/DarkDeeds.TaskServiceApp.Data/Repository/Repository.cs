using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models.Interfaces;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using MongoDB.Driver;

namespace DarkDeeds.TaskServiceApp.Data.Repository
{
    public abstract class Repository<T> : IRepository<T>
        where T: IEntity
    {
        protected readonly IMongoDatabase Database;

        protected Repository()
        {
            // TODO!
            Database = new MongoClient("mongodb://192.168.0.199:27017").GetDatabase("dark-deeds-task-service");
        }
        
        protected abstract IMongoCollection<T> Collection { get; }
        
        public async Task<T> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var cursor = await Collection.FindAsync(x => x.Uid == id);
            return await cursor.SingleOrDefaultAsync();
        }

        public Task<IList<T>> GetBySpecAsync(ISpecification<T> spec)
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));

            return Task.FromResult(spec.Apply(Collection.AsQueryable()).ToList() as IList<T>);
        }

        public async Task<IList<T>> GetListAsync()
        {
            var cursor = await Collection.FindAsync(x => true);
            return await cursor.ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var cursor = await Collection.FindAsync(predicate);
            return await cursor.AnyAsync();
        }

        public async Task SaveAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var exists = await Collection.CountDocumentsAsync(x => x.Uid == entity.Uid) > 0;

            if (!exists)
                await Collection.InsertOneAsync(entity);
            else
                await Collection.ReplaceOneAsync(x => x.Uid == entity.Uid, entity);
        }
        
        public Task SavePropertiesAsync(T entity, params Expression<Func<T, object>>[] properties)
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

            return Collection.UpdateOneAsync(x => x.Uid == entity.Uid, update);
        }

        public Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            var update = Builders<T>.Update.Set(x => x.IsDeleted, true);
            return Collection.UpdateOneAsync(x => x.Uid == id, update);
        }

        public Task DeleteHardAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            return Collection.DeleteOneAsync(x => x.Uid == id);
        }
    }
}