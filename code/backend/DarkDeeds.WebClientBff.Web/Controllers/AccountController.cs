using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.Authentication.Core;
using DarkDeeds.ServiceAuth.Consumers;
using DarkDeeds.ServiceAuth.Dto.Dto;
using DarkDeeds.WebClientBff.UseCases.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBff.Web.Controllers;

[AllowAnonymous]
public class AccountController(
    IAuthServiceApp authServiceApp,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper) : BaseController
{
    [HttpPost(nameof(SignUp))]
    public Task<SignUpResultDto> SignUp(SignUpInfoDto signUpInfo)
    {
        Validate();
        return authServiceApp.SignUpAsync(signUpInfo);
    }

    [HttpPost(nameof(SignIn))]
    public Task<SignInResultDto> SignIn(SignInInfoDto signInInfo)
    {
        Validate();
        return authServiceApp.SignInAsync(signInInfo);
    }

    // TODO: move to auth service
    [HttpGet]
    public Task<CurrentUserDto> Current()
    {
        var user = httpContextAccessor.HttpContext.User;

        var currentUser = user.Identity != null && user.Identity.IsAuthenticated
            ? mapper.Map<CurrentUserDto>(user.ToAuthToken())
            : new CurrentUserDto();

        return Task.FromResult(currentUser);
    }
}
