using System.ComponentModel.DataAnnotations;
using DD.ServiceTask.Domain.Services;
using DD.Shared.Details.Abstractions.Dto;
using DD.Shared.Web;
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
    public async Task<IEnumerable<TaskDto>> Get([Required] DateTime from)
    {
        // await Task.Delay(1000);
        return await taskService.LoadActualTasksAsync(userAuth.UserId(), from);
    }

    [HttpPost]
    public async Task<IEnumerable<TaskDto>> Post([FromBody] ICollection<TaskDto> tasks)
    {
        // await Task.Delay(3000);
        return await taskService.SaveTasksAsync(tasks, userAuth.UserId());
    }
}
