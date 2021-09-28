using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.TelegramClientApp.Entities;
using DarkDeeds.TelegramClientApp.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DarkDeeds.TelegramClientApp.Services.Implementation
{
    public class TestService : ITestService
    {
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1);
        
        private readonly UserManager<UserEntity> _userManager;

        public TestService(UserManager<UserEntity> userManager)
        {
            _userManager = userManager;
        }

        public async Task<int> GetTestChatIdForUser(string username)
        {
            await Semaphore.WaitAsync();

            try
            {
                var user = await _userManager.FindByNameAsync(username);
                var minChatId = await _userManager.Users.Select(x => x.TelegramChatId).MinAsync();
                user.TelegramChatId = minChatId - 1;
                await _userManager.UpdateAsync(user);
                return user.TelegramChatId;
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}