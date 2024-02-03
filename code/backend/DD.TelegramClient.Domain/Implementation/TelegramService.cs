using DD.Shared.Data.Abstractions;
using DD.TelegramClient.Domain.Entities;

namespace DD.TelegramClient.Domain.Implementation;

public interface ITelegramService
{
    Task<string> GenerateKey(string userId, int timeAdjustment);
    Task UpdateChatId(string userChatKey, int chatId);
    Task<string> GetUserId(int chatId);
    Task<int> GetUserTimeAdjustment(int chatId);
}

class TelegramService(IRepository<TelegramUserEntity> telegramUserRepository) : ITelegramService
{
    public async Task<string> GenerateKey(string userId, int timeAdjustment)
    {
        var user = telegramUserRepository.GetAll().FirstOrDefault(x => x.UserId == userId);

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

        await telegramUserRepository.SaveAsync(user);

        return user.TelegramChatKey;
    }

    public async Task UpdateChatId(string userChatKey, int chatId)
    {
        var users = telegramUserRepository.GetAll()
            .Where(x => x.TelegramChatKey == userChatKey || x.TelegramChatId == chatId)
            .ToList();

        foreach (var user in users)
        {
            user.TelegramChatId = user.TelegramChatKey == userChatKey ? chatId : 0;

            await telegramUserRepository.SaveAsync(user);
        }
    }

    public Task<string> GetUserId(int chatId)
    {
        var user = telegramUserRepository.GetAll().Single(x => x.TelegramChatId == chatId);
        return Task.FromResult(user.UserId);
    }

    public Task<int> GetUserTimeAdjustment(int chatId)
    {
        var user = telegramUserRepository.GetAll().Single(x => x.TelegramChatId == chatId);
        return Task.FromResult(user.TelegramTimeAdjustment);
    }
}
