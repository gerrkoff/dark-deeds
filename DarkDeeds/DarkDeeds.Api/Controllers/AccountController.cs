using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.Api.Controllers.Base;
using DarkDeeds.Models.Account;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseUserController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost(nameof(SignUp))]
        public async Task<SignUpResultDto> SignUp(SignUpInfoDto signUpInfo)
        {
            Validate();
            return await _accountService.SignUp(signUpInfo);
        }

        [AllowAnonymous]
        [HttpPost(nameof(SignIn))]
        public async Task<SignInResultDto> SignIn(SignInInfoDto signInInfo)
        {
            Validate();
            return await _accountService.SignIn(signInInfo);
        }

        [HttpGet]
        public CurrentUserDto Current()
        {
            return User.Identity.IsAuthenticated
                ? Mapper.Map<CurrentUserDto>(GetUser())
                : new CurrentUserDto();
        }
	}
}