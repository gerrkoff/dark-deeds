using System.Linq.Expressions;
using DD.Shared.Data;
using DD.TelegramClient.Domain.Entities;
using DD.TelegramClient.Domain.Infrastructure;
using MongoDB.Driver;

namespace DD.TelegramClient.Details.Data;

public sealed class TelegramUserRepository(IMongoDbContext dbContext)
    : BaseRepository<TelegramUserEntity>(dbContext, "telegramUsers"), ITelegramUserRepository
{
    static TelegramUserRepository()
    {
        RegisterDefaultMap<TelegramUserEntity>();
    }

    protected override Expression<Func<TelegramUserEntity, string>> FieldId => x => x.UserId;

    public async Task<TelegramUserEntity?> GetByChatIdAsync(int chatId)
    {
        using var cursor = await Collection
            .FindAsync(x => x.TelegramChatId == chatId);
        return await cursor.SingleOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<TelegramUserEntity>> GetByChatIdOrChatKeyAsync(int chatId, string chatKey)
    {
        using var cursor = await Collection
            .FindAsync(x => x.TelegramChatId == chatId || x.TelegramChatKey == chatKey);
        return await cursor.ToListAsync();
    }

    public async Task<int> GetMinChatIdAsync()
    {
        var cursor = Collection.Find(x => true)
            .SortBy(x => x.TelegramChatId)
            .Limit(1);
        return (await cursor.FirstOrDefaultAsync())?.TelegramChatId ?? 0;
    }
}
