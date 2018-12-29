using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace DarkDeeds.Api.Controllers
{
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment)
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
    }
}