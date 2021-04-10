using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.App.Controllers.Base;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.Dto;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.GetCurrentUser;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.SignIn;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.SignUp;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBffApp.App.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(nameof(SignUp))]
        public Task<SignUpResultDto> SignUp(SignUpInfoDto signUpInfo)
        {
            Validate();
            return _mediator.Send(new SignUpRequestModel(signUpInfo));
        }

        [HttpPost(nameof(SignIn))]
        public Task<SignInResultDto> SignIn(SignInInfoDto signInInfo)
        {
            Validate();
            return _mediator.Send(new SignInRequestModel(signInInfo));
        }

        [HttpGet]
        public Task<CurrentUserDto> Current()
        {
            return _mediator.Send(new GetCurrentUserRequestModel());
        }
    }
}