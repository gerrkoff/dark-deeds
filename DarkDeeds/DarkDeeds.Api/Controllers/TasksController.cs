using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DarkDeeds.Api.Controllers.Base;
using DarkDeeds.Authentication;
using DarkDeeds.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.Infrastructure.Communication.TaskServiceApp.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : BaseController
    {
        private readonly ITaskServiceApp _taskServiceApp;

        public TasksController(ITaskServiceApp taskServiceApp)
        {
            _taskServiceApp = taskServiceApp;
        }

        [HttpGet]
        public Task<IEnumerable<TaskDto>> Get([Required] DateTime from)
        {
            return _taskServiceApp.LoadActualTasksAsync(User.ToAuthToken().UserId, from);
        }
        
        [HttpPost]
        public Task<IEnumerable<TaskDto>> Post([FromBody] ICollection<TaskDto> tasks)
        {
            return _taskServiceApp.SaveTasksAsync(tasks, User.ToAuthToken().UserId);
        }
    }
}