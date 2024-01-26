using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Consumers;
using DD.TaskService.Domain.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBff.Web.Controllers;

public class RecurrencesController(ITaskServiceApp taskServiceApp) : BaseController
{
    [Route("create")]
    [HttpPost]
    public Task<int> Create(int timezoneOffset)
    {
        return taskServiceApp.CreateRecurrencesAsync(timezoneOffset);
    }

    [HttpGet]
    public Task<IEnumerable<PlannedRecurrenceDto>> Get()
    {
        return taskServiceApp.LoadRecurrencesAsync();
    }

    [HttpPost]
    public Task<int> Post([FromBody] ICollection<PlannedRecurrenceDto> recurrences)
    {
        return taskServiceApp.SaveRecurrencesAsync(recurrences);
    }
}
