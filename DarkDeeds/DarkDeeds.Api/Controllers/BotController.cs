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
        private readonly IHubContext<TaskHub> _taskHubContext;
        
        public BotController(IBotProcessMessageService botProcessMessageService, IHubContext<TaskHub> taskHubContext)
        {
            _botProcessMessageService = botProcessMessageService;
            _taskHubContext = taskHubContext;
        }

        [HttpPost]
        public Task Process([FromBody] UpdateDto update)
        {   
            return _botProcessMessageService.ProcessMessageAsync(update, SendUpdateTasks);
        }

        private async void SendUpdateTasks(IEnumerable<TaskDto> tasks)
        {
            await TaskHub.Update(_taskHubContext, tasks);
        }
    }
}