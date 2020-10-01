using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace DarkDeeds.Api.Controllers
{
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return new PhysicalFileResult(
                Path.Combine(_hostingEnvironment.WebRootPath, "index.html"),
                new MediaTypeHeaderValue("text/html")
            );
        }
        
        [Route("api/build-info")]
        [HttpGet]
        public BuildInfoDto BuildInfo()
        {
            return new BuildInfoDto
            {
                Version = Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                    .InformationalVersion
            };
        }
        
        public class BuildInfoDto
        {
            public string Version { get; set; }
        }
    }
}