using DD.ServiceAuth.Domain.Dto;
using DD.ServiceAuth.Domain.Services;
using DD.Shared.Web;
using Microsoft.AspNetCore.Mvc;

namespace DD.ServiceAuth.Details.Web.Controllers;

public class TestController(ITestService testService) : BaseControllerTest
{
    [HttpPost(nameof(CreateTestUser))]
    public Task<TestUserDto> CreateTestUser()
    {
        return testService.CreateTestUserAsync();
    }
}
