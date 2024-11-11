using DD.MobileClient.Domain.Dto;
using DD.MobileClient.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DD.Clients.Details.MobileClient.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/mobile/[controller]")]
public class WatchController(IWatchService watchService) : ControllerBase
{
    [HttpGet("{mobileKey}/widget")]
    public Task<WatchWidgetStatusDto> GetWidget(string mobileKey)
    {
        return watchService.GetWidgetStatus(mobileKey);
    }

    [HttpGet("{mobileKey}/app")]
    public Task<WatchAppStatusDto> GetApp(string mobileKey)
    {
        return watchService.GetAppStatus(mobileKey);
    }
}
