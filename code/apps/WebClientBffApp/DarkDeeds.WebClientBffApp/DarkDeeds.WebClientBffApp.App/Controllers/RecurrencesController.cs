using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Authentication;
using DarkDeeds.WebClientBffApp.App.Controllers.Base;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBffApp.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurrencesController : BaseController
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
            return _taskServiceApp.CreateRecurrencesAsync(timezoneOffset, User.ToAuthToken().UserId);
        }
        
        [HttpGet]
        public async Task<IEnumerable<PlannedRecurrenceDto>> Get()
        {
            return await _taskServiceApp.LoadRecurrencesAsync(User.ToAuthToken().UserId);
        }
        
        [HttpPost]
        public Task<int> Post([FromBody] ICollection<PlannedRecurrenceDto> recurrences)
        {
            return _taskServiceApp.SaveRecurrencesAsync(recurrences, User.ToAuthToken().UserId);
        }
    }
}