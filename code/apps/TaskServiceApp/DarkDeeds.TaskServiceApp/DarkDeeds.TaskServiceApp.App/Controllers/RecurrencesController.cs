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
    public class RecurrencesController : ControllerBase
    {
        private readonly IRecurrenceCreatorService _recurrenceCreatorService;
        private readonly IRecurrenceService _recurrenceService;

        public RecurrencesController(IRecurrenceCreatorService recurrenceCreatorService, IRecurrenceService recurrenceService)
        {
            _recurrenceCreatorService = recurrenceCreatorService;
            _recurrenceService = recurrenceService;
        }

        [Route("create")]
        [HttpPost]
        public Task<int> Create(int timezoneOffset, [Required] string userId)
        {
            return _recurrenceCreatorService.CreateAsync(timezoneOffset, userId);
        }
        
        [HttpGet]
        public Task<IEnumerable<PlannedRecurrenceDto>> Get([Required] string userId)
        {
            return _recurrenceService.LoadAsync(userId);
        }
        
        [HttpPost]
        public Task<int> Post([FromBody] ICollection<PlannedRecurrenceDto> recurrences, [Required] string userId)
        {
            return _recurrenceService.SaveAsync(recurrences, userId);
        }
    }
}