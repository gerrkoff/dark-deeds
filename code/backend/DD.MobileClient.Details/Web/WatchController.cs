using DD.MobileClient.Domain.Dto;
using DD.MobileClient.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DD.MobileClient.Details.Web;

[AllowAnonymous]
[ApiController]
[Route("api/mobile/[controller]")]
public class WatchController(IWatchService watchService) : ControllerBase
{
    [HttpGet("{mobileKey}")]
    public Task<WatchStatusDto> Get(string mobileKey)
    {
        return watchService.GetStatus(mobileKey);
    }
}
