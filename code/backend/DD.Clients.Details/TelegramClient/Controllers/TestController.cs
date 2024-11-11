using DD.Shared.Details.Controllers;
using DD.TelegramClient.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace DD.Clients.Details.TelegramClient.Controllers;

public class TestController(ITestService testService) : BaseControllerTest
{
    [HttpPost(nameof(GetTestChatIdForUser))]
    public Task<int> GetTestChatIdForUser(string userId)
    {
        return testService.GetTestChatIdForUser(userId);
    }
}
