using System.Threading.Tasks;

namespace DarkDeeds.TelegramClient.Services.Interface
{
    public interface ITestService
    {
        Task<int> GetTestChatIdForUser(string username);
    }
}