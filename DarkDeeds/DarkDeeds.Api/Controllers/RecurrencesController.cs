using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.Api.Controllers.Base;
using DarkDeeds.Models;
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
        public Task<int> Create()
        {
            return _recurrenceCreatorService.CreateAsync(GetUser().UserId);
        }
        
        [HttpGet]
        public Task<IEnumerable<PlannedRecurrenceDto>> Get()
        {
            return _recurrenceService.GetRecurrences(GetUser().UserId);
        }
    }
}