using DD.Shared.Web;
using DD.WebClientBff.Domain.Dto;
using DD.WebClientBff.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace DD.WebClientBff.Details.Web;

[ApiController]
[Route("api/web/[controller]")]
public class SettingsController(
    IUserAuth userAuth,
    ISettingsService settingsService)
    : ControllerBase
{
    [HttpGet]
    public Task<SettingsDto> Get()
    {
        return settingsService.LoadAsync(userAuth.UserId());
    }

    [HttpPost]
    public Task Post([FromBody] SettingsDto settings)
    {
        return settingsService.SaveAsync(settings, userAuth.UserId());
    }
}
