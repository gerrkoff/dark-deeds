using System.Linq.Expressions;
using DD.ServiceTask.Domain.Entities.Abstractions;
using DD.ServiceTask.Domain.Infrastructure;
using DD.ServiceTask.Domain.Specifications;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DD.ServiceTask.Details.Data;

internal abstract class Repository<T>(IMongoDbContext dbContext, string tableName) : IRepository<T>
    where T : Entity
{
    private readonly IMongoCollection<T> _collection = dbContext.GetCollection<T>(tableName);

    public async Task<T?> GetByIdAsync(string uid)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uid);

        using var cursor = await _collection.FindAsync(x => x.Uid == uid);
        return await cursor.SingleOrDefaultAsync();
    }

    public async Task<IList<T>> GetBySpecAsync(ISpecification<T> spec)
    {
        var query = spec.Apply(_collection.AsQueryable()) as IMongoQueryable<T>;
        return await query.ToListAsync();
    }

    public async Task<bool> AnyAsync(ISpecification<T> spec)
    {
        var query = spec.Apply(_collection.AsQueryable()) as IMongoQueryable<T>;
        return await query.AnyAsync();
    }

    public async Task UpsertAsync(T entity)
    {
        await _collection.ReplaceOneAsync(x => x.Uid == entity.Uid, entity, new ReplaceOptions
        {
            IsUpsert = true,
        });
    }

    public async Task<(bool, T?)> TryUpdateVersionAsync(T entity)
    {
        var version = entity.Version;
        entity.Version++;

        var filter = Builders<T>.Filter
            .Where(x => x.Uid == entity.Uid && x.Version == version);

        var result = await _collection.ReplaceOneAsync(filter, entity, new ReplaceOptions
        {
            IsUpsert = false,
        });

        if (result.MatchedCount == 1)
            return (true, null);

        entity.Version--;
        var current = await GetByIdAsync(entity.Uid);
        return (false, current);
    }

    public async Task<(bool, T?)> TryUpdateVersionPropsAsync(T entity, params Expression<Func<T, object>>[] properties)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entity.Uid);

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
            IsUpsert = false,
        });

        if (result.MatchedCount == 1)
            return (true, null);

        entity.Version--;
        var current = await GetByIdAsync(entity.Uid);
        return (false, current);
    }

    public async Task<bool> DeleteAsync(string uid)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uid);

        var update = Builders<T>.Update.Set(x => x.IsDeleted, true);

        var result = await _collection.UpdateOneAsync(x => x.Uid == uid, update, new UpdateOptions
        {
            IsUpsert = false,
        });

        return result.ModifiedCount == 1;
    }

    public async Task<bool> DeleteHardAsync(string uid)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uid);

        var result = await _collection.DeleteOneAsync(x => x.Uid == uid);
        return result.DeletedCount == 1;
    }

    protected static void RegisterDefaultMap<TEntity>()
    {
        BsonClassMap.RegisterClassMap<TEntity>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
        });
    }
}