using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.Api.Controllers.Base;
using DarkDeeds.Models.Account;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : BaseUserController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<RegisterResultDto> Register(RegisterInfoDto registerInfo)
        {
            Validate();
            return await _accountService.Register(registerInfo);
        }

        [HttpPost]
        public async Task<LoginResultDto> Login(LoginInfoDto loginInfo)
        {
            Validate();
            return await _accountService.Login(loginInfo);
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