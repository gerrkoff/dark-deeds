using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Recurrences.Create;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Recurrences.Load;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Recurrences.Save;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBffApp.Web.Controllers
{
    public class RecurrencesController : BaseController
    {
        private readonly IMediator _mediator;

        public RecurrencesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("create")]
        [HttpPost]
        public Task<int> Create(int timezoneOffset)
        {
            return _mediator.Send(new CreateRequestModel(timezoneOffset));
        }
        
        [HttpGet]
        public Task<IEnumerable<PlannedRecurrenceDto>> Get()
        {
            return _mediator.Send(new LoadRequestModel());
        }
        
        [HttpPost]
        public Task<int> Post([FromBody] ICollection<PlannedRecurrenceDto> recurrences)
        {
            return _mediator.Send(new SaveRequestModel(recurrences));
        }
    }
}