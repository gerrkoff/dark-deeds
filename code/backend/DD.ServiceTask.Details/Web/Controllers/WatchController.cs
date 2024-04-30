using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DD.ServiceTask.Details.Web.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/mobile/[controller]")]
public class WatchController(
    ILogger<WatchController> logger) : ControllerBase
{
    [HttpGet("{mobileUserId}")]
    public Resp Get(string mobileUserId)
    {
#pragma warning disable CA1848
        logger.LogInformation("Get watch for {MobileUserId}", mobileUserId);
#pragma warning restore CA1848
        return new Resp
        {
            Header = "6 (8) remaining",
            Main = "Do some work",
            Support = "Do some not required work",
        };
    }
}

#pragma warning disable SA1402
public class Resp
#pragma warning restore SA1402
{
    public required string Header { get; set; }

    public required string Main { get; set; }

    public required string Support { get; set; }
}
