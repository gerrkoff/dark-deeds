using DD.Shared.Web;
using DD.WebClientBff.Domain.Dto;
using DD.WebClientBff.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace DD.Shared.Details.WebClientBff.Controllers;

[ApiController]
[Route("api/web/[controller]")]
public class SettingsController(
    IUserAuth userAuth,
    IUserSettingsService userSettingsService)
    : ControllerBase
{
    [HttpGet]
    public async Task<UserSettingsDto> Get()
    {
        await Task.Delay(3000);
        return await userSettingsService.LoadAsync(userAuth.UserId());
    }

    [HttpPost]
    public async Task Post([FromBody] UserSettingsDto userSettings)
    {
        await Task.Delay(3000);
        userSettingsService.SaveAsync(userSettings, userAuth.UserId());
    }
}
