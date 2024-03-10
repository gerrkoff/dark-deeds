using DD.Shared.Web;
using DD.TelegramClient.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace DD.TelegramClient.Details.Web.Controllers;

public class TestController(ITestService testService) : BaseControllerTest
{
    [HttpPost(nameof(GetTestChatIdForUser))]
    public Task<int> GetTestChatIdForUser(string userId)
    {
        return testService.GetTestChatIdForUser(userId);
    }
}
