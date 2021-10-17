using System.Threading.Tasks;

namespace DarkDeeds.TelegramClientApp.Services.Interface
{
    public interface ITestService
    {
        Task<int> GetTestChatIdForUser(string username);
    }
}