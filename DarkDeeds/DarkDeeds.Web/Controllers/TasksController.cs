using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DarkDeeds.Api.Controllers.Base;
using DarkDeeds.Services.Dto;
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
        public Task<IEnumerable<TaskDto>> Get([Required] DateTime? from)
        {
            from = from.GetValueOrDefault().ToUniversalTime();
            return _taskService.LoadActualTasksAsync(GetUser().UserId, from.GetValueOrDefault());
        }
        
        [HttpPost]
        public Task<IEnumerable<TaskDto>> Post([FromBody] ICollection<TaskDto> tasks)
        {
            return _taskService.SaveTasksAsync(tasks, GetUser().UserId);
        }
    }
}