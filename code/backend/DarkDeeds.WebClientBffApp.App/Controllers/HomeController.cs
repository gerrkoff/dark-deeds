using DarkDeeds.Common.Misc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBffApp.App.Controllers
{
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        [Route("api/build-info")]
        [HttpGet]
        public BuildInfo BuildInfo()
        {
            return new(typeof(Startup), null);
        }
    }
}