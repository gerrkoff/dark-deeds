using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DD.TaskService.Domain.Entities.Abstractions;
using DD.TaskService.Domain.Infrastructure;
using DD.TaskService.Domain.Specifications;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DarkDeeds.ServiceTask.Data;

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

        using var cursor = await _collection.FindAsync(x => x.Uid == uid);
        return await cursor.SingleOrDefaultAsync();
    }

    public async Task<IList<T>> GetBySpecAsync(ISpecification<T> spec)
    {
        if (spec == null)
            throw new ArgumentNullException(nameof(spec));

        var query = spec.Apply(_collection.AsQueryable()) as IMongoQueryable<T>;
        return await query.ToListAsync();
    }

    public async Task<bool> AnyAsync(ISpecification<T> spec)
    {
        if (spec == null)
            throw new ArgumentNullException(nameof(spec));

        var query = spec.Apply(_collection.AsQueryable()) as IMongoQueryable<T>;
        return await query.AnyAsync();
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

    public async Task<(bool, T)> TryUpdateVersionPropsAsync(T entity, params Expression<Func<T, object>>[] properties)
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

    public async Task<bool> DeleteAsync(string uid)
    {
        if (string.IsNullOrWhiteSpace(uid))
            throw new ArgumentNullException(nameof(uid));

        var update = Builders<T>.Update.Set(x => x.IsDeleted, true);

        var result = await _collection.UpdateOneAsync(x => x.Uid == uid, update, new UpdateOptions
        {
            IsUpsert = false
        });

        return result.ModifiedCount == 1;
    }

    public async Task<bool> DeleteHardAsync(string uid)
    {
        if (string.IsNullOrWhiteSpace(uid))
            throw new ArgumentNullException(nameof(uid));

        var result = await _collection.DeleteOneAsync(x => x.Uid == uid);
        return result.DeletedCount == 1;
    }
}
