using System.Threading.Tasks;
using DarkDeeds.CommonWeb;
using DarkDeeds.TelegramClientApp.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.TelegramClientApp.Web.Controllers
{
    public class TestController : BaseControllerTest
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
}