using DD.App.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DD.App.Controllers;

[AllowAnonymous]
public class HomeController : ControllerBase
{
    [Route("api/be/build-info")]
    [HttpGet]
    public BuildInfoDto BuildInfo()
    {
        return new(typeof(Startup), null);
    }
}
