using System.Threading.Tasks;
using DarkDeeds.Authentication;
using DarkDeeds.Authentication.Models;
using DarkDeeds.WebClientBffApp.App.Controllers.Base;
using DarkDeeds.WebClientBffApp.App.Dto;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBffApp.App.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IAuthServiceApp _authServiceApp;

        public AccountController(IAuthServiceApp authServiceApp)
        {
            _authServiceApp = authServiceApp;
        }

        [HttpPost(nameof(SignUp))]
        public Task<SignUpResultDto> SignUp(SignUpInfoDto signUpInfo)
        {
            Validate();
            return _authServiceApp.SignUpAsync(signUpInfo);
        }

        [HttpPost(nameof(SignIn))]
        public Task<SignInResultDto> SignIn(SignInInfoDto signInInfo)
        {
            Validate();
            return _authServiceApp.SignInAsync(signInInfo);
        }

        [HttpGet]
        public CurrentUserDto Current()
        {
            return User.Identity != null && User.Identity.IsAuthenticated
                ? ToDto(User.ToAuthToken())
                : new CurrentUserDto();

            CurrentUserDto ToDto(AuthToken user) => new()
            {
                Username = user.DisplayName,
                UserAuthenticated = !string.IsNullOrEmpty(user.Username)
            };
        }
    }
}