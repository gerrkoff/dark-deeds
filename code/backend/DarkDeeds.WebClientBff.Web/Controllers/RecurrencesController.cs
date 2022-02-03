using System.Collections.Generic;
using System.Threading.Tasks;
using DarkDeeds.WebClientBff.Infrastructure.Communication.TaskServiceApp.Dto;
using DarkDeeds.WebClientBff.UseCases.Handlers.Recurrences.Create;
using DarkDeeds.WebClientBff.UseCases.Handlers.Recurrences.Load;
using DarkDeeds.WebClientBff.UseCases.Handlers.Recurrences.Save;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBff.Web.Controllers
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