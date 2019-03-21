using System;
using System.Threading.Tasks;

namespace DarkDeeds.Services.Interface
{
    public interface ITelegramService
    {
        Task<Guid> GenerateKey(string userId);
        Task UpdateChatId(Guid userChatKey, int chatId);
        Task<string> GetUserId(int chatId);
    }
}