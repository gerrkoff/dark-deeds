using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Models.Dto;
using DarkDeeds.TaskServiceApp.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.TaskServiceApp.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public Task<IEnumerable<TaskDto>> Get([Required] DateTime from, [Required] string userId)
        {
            return _taskService.LoadActualTasksAsync(userId, from);
        }
        
        [HttpGet(nameof(ByDate))]
        public Task<IEnumerable<TaskDto>> ByDate([Required] DateTime from, [Required] DateTime to, [Required] string userId)
        {
            return _taskService.LoadTasksByDateAsync(userId, from, to);
        }
        
        [HttpPost]
        public Task<IEnumerable<TaskDto>> Post([FromBody] ICollection<TaskDto> tasks, [Required] string userId)
        {
            return _taskService.SaveTasksAsync(tasks, userId);
        }
    }
}