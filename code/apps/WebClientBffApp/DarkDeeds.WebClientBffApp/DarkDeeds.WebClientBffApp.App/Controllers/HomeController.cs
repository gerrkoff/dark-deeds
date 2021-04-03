using System.Reflection;
using DarkDeeds.WebClientBffApp.App.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBffApp.App.Controllers
{
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        [Route("api/build-info")]
        [HttpGet]
        public BuildInfoDto BuildInfo()
        {
            return new()
            {
                Version = Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                    .InformationalVersion
            };
        }
    }
}