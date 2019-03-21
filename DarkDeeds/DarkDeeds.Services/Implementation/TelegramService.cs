using System;
using System.Threading.Tasks;
using DarkDeeds.Services.Interface;

namespace DarkDeeds.Services.Implementation
{
    public class TelegramService : ITelegramService
    {
        public Task<Guid> GenerateKey(string userId)
        {
            return Task.FromResult(Guid.NewGuid());
        }

        public Task UpdateChatId(Guid userChatKey, int chatId)
        {
            return Task.CompletedTask;
        }

        public Task<string> GetUserId(int chatId)
        {
            return Task.FromResult("");
        }
    }
}