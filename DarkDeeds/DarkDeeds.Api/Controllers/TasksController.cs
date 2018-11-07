using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Models;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IEnumerable<TaskDto>> Get()
        {
            return await _taskService.LoadTasksAsync();
        }
        
        [HttpPost]
        public async Task<IEnumerable<TaskDto>> Post([FromBody] ICollection<TaskDto> tasks)
        {
            return await _taskService.SaveTasksAsync(tasks);
        }
    }
}