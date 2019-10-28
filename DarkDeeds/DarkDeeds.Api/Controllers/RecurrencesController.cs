using System.Threading.Tasks;
using DarkDeeds.Api.Controllers.Base;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurrencesController : BaseUserController
    {
        private readonly IRecurrenceCreatorService _recurrenceCreatorService;

        public RecurrencesController(IRecurrenceCreatorService recurrenceCreatorService)
        {
            _recurrenceCreatorService = recurrenceCreatorService;
        }

        [Route("create")]
        [HttpPost]
        public Task<int> Create()
        {
            return _recurrenceCreatorService.CreateAsync(GetUser().UserId);
        }
    }
}