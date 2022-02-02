using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.TelegramClientApp.Web.Controllers
{
    [ApiController]
    [Route("api/tlgm/[controller]")]
    public abstract class BaseController : Controller
    {
    }
}