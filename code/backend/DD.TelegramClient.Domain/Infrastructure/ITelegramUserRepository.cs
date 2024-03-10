using DD.TelegramClient.Domain.Entities;

namespace DD.TelegramClient.Domain.Infrastructure;

public interface ITelegramUserRepository
{
    Task<TelegramUserEntity?> GetByIdAsync(string userId);

    Task<TelegramUserEntity?> GetByChatIdAsync(int chatId);

    Task<IReadOnlyCollection<TelegramUserEntity>> GetByChatIdOrChatKeyAsync(int chatId, string chatKey);

    Task<int> GetMinChatIdAsync();

    Task UpsertAsync(TelegramUserEntity entity);
}
