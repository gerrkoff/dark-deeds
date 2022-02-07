using System.Threading.Tasks;
using DarkDeeds.WebClientBff.Services.Dto;
using DarkDeeds.WebClientBff.UseCases.Handlers.Settings.Load;
using DarkDeeds.WebClientBff.UseCases.Handlers.Settings.Save;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBff.Web.Controllers
{
    public class SettingsController : BaseController
    {
        private readonly IMediator _mediator;

        public SettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public Task<SettingsDto> Get()
        {
            return _mediator.Send(new LoadRequestModel());
        }
        
        [HttpPost]
        public Task Post([FromBody] SettingsDto settings)
        {
            return _mediator.Send(new SaveRequestModel(settings));
        }
    }
}