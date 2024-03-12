using DD.TelegramClient.Domain.Entities;
using DD.TelegramClient.Domain.Infrastructure;

namespace DD.TelegramClient.Domain.Services;

public interface ITelegramService
{
    Task<string> GenerateKey(string userId, int timeAdjustment);

    Task UpdateChatId(string userChatKey, int chatId);

    Task<string> GetUserId(int chatId);

    Task<int> GetUserTimeAdjustment(int chatId);
}

internal sealed class TelegramService(ITelegramUserRepository telegramUserRepository) : ITelegramService
{
    public async Task<string> GenerateKey(string userId, int timeAdjustment)
    {
        var user = await telegramUserRepository.GetByIdAsync(userId);

        if (user == null)
        {
            user = new TelegramUserEntity
            {
                UserId = userId,
                TelegramChatKey = Guid.NewGuid().ToString(),
                TelegramTimeAdjustment = timeAdjustment,
            };
        }
        else
        {
            user.TelegramChatKey = Guid.NewGuid().ToString();
            user.TelegramTimeAdjustment = timeAdjustment;
        }

        await telegramUserRepository.UpsertAsync(user);

        return user.TelegramChatKey;
    }

    public async Task UpdateChatId(string userChatKey, int chatId)
    {
        var users = await telegramUserRepository.GetByChatIdOrChatKeyAsync(chatId, userChatKey);

        foreach (var user in users)
        {
            user.TelegramChatId = user.TelegramChatKey == userChatKey ? chatId : 0;

            await telegramUserRepository.UpsertAsync(user);
        }
    }

    public async Task<string> GetUserId(int chatId)
    {
        var user = await telegramUserRepository.GetByChatIdAsync(chatId)
                   ?? throw new InvalidOperationException($"User with chat id {chatId} not found");
        return user.UserId;
    }

    public async Task<int> GetUserTimeAdjustment(int chatId)
    {
        var user = await telegramUserRepository.GetByChatIdAsync(chatId)
                     ?? throw new InvalidOperationException($"User with chat id {chatId} not found");
        return user.TelegramTimeAdjustment;
    }
}
