using DarkDeeds.Models.Telegram;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class BotController : ControllerBase
    {
        private readonly IBotProcessMessageService _botProcessMessageService;
        
        public BotController(IBotProcessMessageService botProcessMessageService)
        {
            _botProcessMessageService = botProcessMessageService;
        }

        public void ProcessMessage(UpdateDto update)
        {
            _botProcessMessageService.ProcessMessage(update);
        } 
    }
}