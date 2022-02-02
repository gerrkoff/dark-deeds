using DarkDeeds.Common.Misc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Backend.App.Controllers
{
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        [Route("api/be/build-info")]
        [HttpGet]
        public BuildInfo BuildInfo()
        {
            return new(typeof(Startup), null);
        }
    }
}