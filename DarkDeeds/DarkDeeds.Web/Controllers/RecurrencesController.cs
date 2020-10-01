using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Api.Controllers.Base;
using DarkDeeds.Models.Dto;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurrencesController : BaseUserController
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
        public Task<int> Create(int timezoneOffset)
        {
            return _recurrenceCreatorService.CreateAsync(timezoneOffset, GetUser().UserId);
        }
        
        [HttpGet]
        public Task<IEnumerable<PlannedRecurrenceDto>> Get()
        {
            return _recurrenceService.LoadAsync(GetUser().UserId);
        }
        
        [HttpPost]
        public Task<int> Post([FromBody] ICollection<PlannedRecurrenceDto> recurrences)
        {
            return _recurrenceService.SaveAsync(recurrences, GetUser().UserId);
        }
    }
}