using System;
using System.Threading.Tasks;

namespace DarkDeeds.Services.Interface
{
    public interface ITelegramService
    {
        Task<string> GenerateKey(string userId);
        Task UpdateChatId(string userChatKey, int chatId);
        Task<string> GetUserId(int chatId);
    }
}