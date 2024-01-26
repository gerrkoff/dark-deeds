using System.Threading.Tasks;
using DD.TelegramClient.Domain.Dto;
using DD.TelegramClient.Domain.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.TelegramClient.Web.Controllers
{
    [AllowAnonymous]
    public class BotController : ControllerBase
    {
        private readonly IBotProcessMessageService _botProcessMessageService;

        public BotController(IBotProcessMessageService botProcessMessageService)
        {
            _botProcessMessageService = botProcessMessageService;
        }

        [HttpPost]
        public Task Process([FromBody] UpdateDto update)
        {
            return _botProcessMessageService.ProcessMessageAsync(update);
        }
    }
}
