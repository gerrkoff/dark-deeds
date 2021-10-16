using System.Threading.Tasks;
using DarkDeeds.TelegramClientApp.Services.Dto;
using DarkDeeds.TelegramClientApp.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.TelegramClientApp.App.Controllers
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