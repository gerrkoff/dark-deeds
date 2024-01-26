using System.Threading.Tasks;
using DarkDeeds.Common.Web;
using DD.TelegramClient.Domain.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.TelegramClient.Web.Controllers
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
