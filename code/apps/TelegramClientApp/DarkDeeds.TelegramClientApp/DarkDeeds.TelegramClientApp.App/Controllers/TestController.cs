using System.Threading.Tasks;
using DarkDeeds.TelegramClientApp.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.TelegramClientApp.App.Controllers
{
#if DEBUG
    [Route("test")]
    [AllowAnonymous]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        [HttpPost(nameof(GetTestChatIdForUser))]
        public Task<int> GetTestChatIdForUser(string username)
        {
            return _testService.GetTestChatIdForUser(username);
        }
    }
#endif
}