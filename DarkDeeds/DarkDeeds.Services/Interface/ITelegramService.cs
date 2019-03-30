using System;
using System.Threading.Tasks;

namespace DarkDeeds.Services.Interface
{
    public interface ITelegramService
    {
        Task<string> GenerateKey(string userId, int timeAdjustment);
        Task UpdateChatId(string userChatKey, int chatId);
        Task<string> GetUserId(int chatId);
        Task<int> GetUserTimeAdjustment(int chatId);
    }
}