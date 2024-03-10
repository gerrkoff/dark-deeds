using System.Linq.Expressions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DD.Shared.Data;

public abstract class BaseRepository<T>(IMongoDbContext dbContext, string tableName)
{
    protected IMongoCollection<T> Collection => dbContext.GetCollection<T>(tableName);

    protected abstract Expression<Func<T, string>> FieldId { get; }

    public async Task<T?> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using var cursor = await Collection.FindAsync(ByIdFilter(id));
        return await cursor.SingleOrDefaultAsync();
    }

    public async Task UpsertAsync(T entity)
    {
        await Collection.ReplaceOneAsync(ByIdFilter(entity), entity, new ReplaceOptions
        {
            IsUpsert = true,
        });
    }

    public async Task<bool> DeleteHardAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var result = await Collection.DeleteOneAsync(ByIdFilter(id));
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

    private FilterDefinition<T> ByIdFilter(string id)
    {
        return Builders<T>.Filter.Eq(FieldId, id);
    }

    private FilterDefinition<T> ByIdFilter(T entity)
    {
        var id = FieldId.Compile()(entity);
        return Builders<T>.Filter.Eq(FieldId, id);
    }
}
