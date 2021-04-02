using System.Threading.Tasks;
using DarkDeeds.AuthServiceApp.App.Controllers.Base;
using DarkDeeds.AuthServiceApp.Services.Dto;
using DarkDeeds.AuthServiceApp.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.AuthServiceApp.App.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost(nameof(SignUp))]
        public Task<SignUpResultDto> SignUp(SignUpInfoDto signUpInfo)
        {
            Validate();
            return _authService.SignUpAsync(signUpInfo);
        }

        [HttpPost(nameof(SignIn))]
        public Task<SignInResultDto> SignIn(SignInInfoDto signInInfo)
        {
            Validate();
            return _authService.SignInAsync(signInInfo);
        }
	}
}