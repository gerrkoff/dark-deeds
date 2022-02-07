using System.Threading.Tasks;
using DarkDeeds.WebClientBff.Infrastructure.Communication.AuthServiceApp.Dto;
using DarkDeeds.WebClientBff.UseCases.Dto;
using DarkDeeds.WebClientBff.UseCases.Handlers.Account.GetCurrentUser;
using DarkDeeds.WebClientBff.UseCases.Handlers.Account.SignIn;
using DarkDeeds.WebClientBff.UseCases.Handlers.Account.SignUp;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBff.Web.Controllers
{
    [AllowAnonymous]
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