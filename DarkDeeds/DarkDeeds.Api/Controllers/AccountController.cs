using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.Api.Controllers.Base;
using DarkDeeds.Models.Account;
using DarkDeeds.Services.Interface;
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

        [HttpPost(nameof(SignUp))]
        public async Task<RegisterResultDto> SignUp(RegisterInfoDto registerInfo)
        {
            Validate();
            return await _accountService.SignUp(registerInfo);
        }

        [HttpPost(nameof(SignIn))]
        public async Task<LoginResultDto> SignIn(LoginInfoDto loginInfo)
        {
            Validate();
            return await _accountService.SignIn(loginInfo);
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