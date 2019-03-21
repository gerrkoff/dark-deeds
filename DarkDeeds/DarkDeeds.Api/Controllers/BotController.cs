using DarkDeeds.BotIntegration.Dto;
using DarkDeeds.BotIntegration.Interface;
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
        public void Process([FromBody] UpdateDto update)
        {
            _botProcessMessageService.ProcessMessage(update);
        }
    }
}