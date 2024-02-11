using DD.Shared.Data.Abstractions;
using DD.TelegramClient.Domain.Entities;

namespace DD.TelegramClient.Domain.Implementation;

public interface ITestService
{
    Task<int> GetTestChatIdForUser(string userId);
}

internal sealed class TestService(IRepository<TelegramUserEntity> telegramUserRepository) : ITestService
{
    private static readonly SemaphoreSlim Semaphore = new(1);

    public async Task<int> GetTestChatIdForUser(string userId)
    {
        await Semaphore.WaitAsync();

        try
        {
            var minChatId = telegramUserRepository.GetAll().Select(x => x.TelegramChatId).DefaultIfEmpty().Min();
            var user = new TelegramUserEntity
            {
                TelegramChatId = minChatId - 1,
                TelegramChatKey = string.Empty,
                UserId = userId,
            };
            await telegramUserRepository.SaveAsync(user);
            return user.TelegramChatId;
        }
        finally
        {
            Semaphore.Release();
        }
    }
}
