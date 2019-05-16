using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Api.Controllers.Base;
using DarkDeeds.Models;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : BaseUserController
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IEnumerable<TaskDto>> Get()
        {
            // TODO: pass from date from client
            return await _taskService.LoadActualTasksAsync(GetUser().UserId, DateTime.UtcNow);
        }
        
        [HttpPost]
        public async Task<IEnumerable<TaskDto>> Post([FromBody] ICollection<TaskDto> tasks)
        {
            return await _taskService.SaveTasksAsync(tasks, GetUser().UserId);
        }
    }
}