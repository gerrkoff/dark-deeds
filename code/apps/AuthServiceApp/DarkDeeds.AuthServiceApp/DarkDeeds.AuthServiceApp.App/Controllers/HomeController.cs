using DarkDeeds.Common.Misc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.AuthServiceApp.App.Controllers
{
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        [Route("api/build-info")]
        [HttpGet]
        public BuildInfo BuildInfo()
        {
            return new(typeof(Startup), typeof(Contract.AuthService));
        }
    }
}
