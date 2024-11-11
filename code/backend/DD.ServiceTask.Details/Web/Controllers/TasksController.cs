using System.ComponentModel.DataAnnotations;
using DD.ServiceTask.Domain.Services;
using DD.Shared.Details.Abstractions.Dto;
using DD.Shared.Details.Services;
using Microsoft.AspNetCore.Mvc;

namespace DD.ServiceTask.Details.Web.Controllers;

[ApiController]
[Route("api/task/[controller]")]
public class TasksController(
    IUserAuth userAuth,
    ITaskService taskService)
    : ControllerBase
{
    [HttpGet]
    public Task<IEnumerable<TaskDto>> Get([Required] DateTime from)
    {
        return taskService.LoadActualTasksAsync(userAuth.UserId(), from);
    }

    [HttpPost]
    public Task<IEnumerable<TaskDto>> Post([FromBody] ICollection<TaskDto> tasks)
    {
        return taskService.SaveTasksAsync(tasks, userAuth.UserId());
    }
}
