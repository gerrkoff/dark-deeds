using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.App.Controllers.Base;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp.Dto;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Telegram.Start;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBffApp.App.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TelegramController : BaseController
    {
        private readonly IMediator _mediator;

        public TelegramController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public Task<TelegramStartDto> Start(int timezoneOffset)
        {
            return _mediator.Send(new StartRequestModel(timezoneOffset));
        }
    }
}