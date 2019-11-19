using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Api.Hubs;
using DarkDeeds.BotIntegration.Dto;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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
        public Task Process([FromBody] UpdateDto update)
        {   
            return _botProcessMessageService.ProcessMessageAsync(update);
        }
    }
}