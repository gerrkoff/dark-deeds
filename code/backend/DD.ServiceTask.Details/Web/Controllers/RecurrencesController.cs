using DD.ServiceTask.Domain.Dto;
using DD.ServiceTask.Domain.Services;
using DD.Shared.Web;
using Microsoft.AspNetCore.Mvc;

namespace DD.ServiceTask.Details.Web.Controllers;

[ApiController]
[Route("api/task/[controller]")]
public class RecurrencesController(
    IUserAuth userAuth,
    IRecurrenceService recurrenceService,
    IRecurrenceCreatorService recurrenceCreatorService)
    : ControllerBase
{
    [Route("create")]
    [HttpPost]
    public Task<int> Create(int timezoneOffset)
    {
        return recurrenceCreatorService.CreateAsync(timezoneOffset, userAuth.UserId());
    }

    [HttpGet]
    public Task<IEnumerable<PlannedRecurrenceDto>> Get()
    {
        return recurrenceService.LoadAsync(userAuth.UserId());
    }

    [HttpPost]
    public Task<int> Post([FromBody] ICollection<PlannedRecurrenceDto> recurrences)
    {
        return recurrenceService.SaveAsync(recurrences, userAuth.UserId());
    }
}
