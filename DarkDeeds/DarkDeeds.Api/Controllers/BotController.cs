using DarkDeeds.Models.Bot;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Api.Controllers
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
        public string Process([FromBody] UpdateDto update)
        {
            _botProcessMessageService.ProcessMessage(update);
            return "Bot";
        } 
    }
}