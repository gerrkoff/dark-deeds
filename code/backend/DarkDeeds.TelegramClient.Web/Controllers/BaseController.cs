using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.TelegramClient.Web.Controllers
{
    [ApiController]
    [Route("api/tlgm/[controller]")]
    public abstract class BaseController : Controller
    {
    }
}