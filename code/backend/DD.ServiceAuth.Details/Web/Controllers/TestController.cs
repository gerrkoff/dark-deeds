using DD.ServiceAuth.Domain.Dto;
using DD.ServiceAuth.Domain.Services;
using DD.Shared.Details.Controllers;
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
