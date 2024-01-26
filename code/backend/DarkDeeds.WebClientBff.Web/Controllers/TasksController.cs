using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Consumers;
using DD.TaskService.Domain.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBff.Web.Controllers;

public class TasksController(ITaskServiceApp taskServiceApp) : BaseController
{
    [HttpGet]
    public Task<IEnumerable<TaskDto>> Get([Required] DateTime from)
    {
        return taskServiceApp.LoadActualTasksAsync(from);
    }

    // TODO: use separate dto?
    [HttpPost]
    public Task<IEnumerable<TaskDto>> Post([FromBody] ICollection<TaskDto> tasks)
    {
        return taskServiceApp.SaveTasksAsync(tasks);
    }
}
