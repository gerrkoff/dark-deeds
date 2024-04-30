using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DD.ServiceTask.Details.Web.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/mobile/[controller]")]
public class WatchController : ControllerBase
{
    [HttpGet("{mobileUserId}")]
    public Resp Get(string mobileUserId)
    {
        return new Resp
        {
            Header = "6 (8) remaining",
            Main = "Do some work",
            Support = "Do some not required work",
        };
    }
}

public class Resp
{
    public string Header { get; set; }

    public string Main { get; set; }

    public string Support { get; set; }
}
