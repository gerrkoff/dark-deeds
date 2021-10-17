using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.App.Controllers.Base;
using DarkDeeds.WebClientBffApp.Services.Dto;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Settings.Load;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Settings.Save;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBffApp.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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