using DD.Shared.Details.Services;
using DD.WebClientBff.Domain.Dto;
using DD.WebClientBff.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace DD.Clients.Details.WebClientBff.Controllers;

[ApiController]
[Route("api/web/[controller]")]
public class SettingsController(
    IUserAuth userAuth,
    IUserSettingsService userSettingsService)
    : ControllerBase
{
    [HttpGet]
    public Task<UserSettingsDto> Get()
    {
        return userSettingsService.LoadAsync(userAuth.UserId());
    }

    [HttpPost]
    public Task Post([FromBody] UserSettingsDto userSettings)
    {
        return userSettingsService.SaveAsync(userSettings, userAuth.UserId());
    }
}
