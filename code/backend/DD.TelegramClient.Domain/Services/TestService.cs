using DD.TelegramClient.Domain.Entities;
using DD.TelegramClient.Domain.Infrastructure;

namespace DD.TelegramClient.Domain.Services;

public interface ITestService
{
    Task<int> GetTestChatIdForUser(string userId);
}

internal sealed class TestService(ITelegramUserRepository telegramUserRepository) : ITestService
{
    private static readonly SemaphoreSlim Semaphore = new(1);

    public async Task<int> GetTestChatIdForUser(string userId)
    {
        await Semaphore.WaitAsync();

        try
        {
            var minChatId = await telegramUserRepository.GetMinChatIdAsync();
            var user = new TelegramUserEntity
            {
                TelegramChatId = minChatId - 1,
                TelegramChatKey = string.Empty,
                UserId = userId,
            };
            await telegramUserRepository.UpsertAsync(user);
            return user.TelegramChatId;
        }
        finally
        {
            Semaphore.Release();
        }
    }
}
