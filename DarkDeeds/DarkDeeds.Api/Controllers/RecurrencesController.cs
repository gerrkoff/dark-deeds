using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Api.Controllers.Base;
using DarkDeeds.Infrastructure.Communication;
using DarkDeeds.Infrastructure.Communication.Dto;
using DarkDeeds.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurrencesController : BaseUserController
    {
        private readonly ITaskServiceApp _taskServiceApp;

        public RecurrencesController(ITaskServiceApp taskServiceApp)
        {
            _taskServiceApp = taskServiceApp;
        }

        [Route("create")]
        [HttpPost]
        public Task<int> Create(int timezoneOffset)
        {
            return _taskServiceApp.CreateRecurrencesAsync(timezoneOffset, GetUser().UserId);
        }
        
        [HttpGet]
        public Task<IEnumerable<PlannedRecurrenceDto>> Get()
        {
            return _taskServiceApp.LoadRecurrencesAsync(GetUser().UserId);
        }
        
        [HttpPost]
        public Task<int> Post([FromBody] ICollection<PlannedRecurrenceDto> recurrences)
        {
            return _taskServiceApp.SaveRecurrencesAsync(recurrences, GetUser().UserId);
        }
    }
}