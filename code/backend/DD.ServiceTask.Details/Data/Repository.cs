using System.Linq.Expressions;
using DD.ServiceTask.Domain.Entities.Abstractions;
using DD.ServiceTask.Domain.Infrastructure;
using DD.ServiceTask.Domain.Specifications;
using DD.Shared.Details.Data;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DD.ServiceTask.Details.Data;

public abstract class Repository<T>(IMongoDbContext dbContext, string tableName)
    : BaseRepository<T>(dbContext, tableName), IRepository<T>
    where T : Entity
{
    protected override Expression<Func<T, string>> FieldId => x => x.Uid;

    public async Task<IList<T>> GetBySpecAsync(ISpecification<T> spec)
    {
        var query = spec.Apply(Collection.AsQueryable()) as IMongoQueryable<T>;
        return await query.ToListAsync();
    }

    public async Task<bool> AnyAsync(ISpecification<T> spec)
    {
        var query = spec.Apply(Collection.AsQueryable()) as IMongoQueryable<T>;
        return await query.AnyAsync();
    }

    public async Task<(bool, T?)> TryUpdateVersionAsync(T entity)
    {
        var version = entity.Version;
        entity.Version++;

        var filter = Builders<T>.Filter
            .Where(x => x.Uid == entity.Uid && x.Version == version);

        var result = await Collection.ReplaceOneAsync(filter, entity, new ReplaceOptions
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

        update = update.Set(x => x.Version, entity.Version);

        var result = await Collection.UpdateOneAsync(filter, update, new UpdateOptions
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

        var update = Builders<T>.Update.Set(x => x.DeletedAt, DateTimeOffset.UtcNow);

        var result = await Collection.UpdateOneAsync(x => x.Uid == uid, update, new UpdateOptions
        {
            IsUpsert = false,
        });

        return result.ModifiedCount == 1;
    }
}
