using AutoMapper;
using DD.ServiceAuth.Domain.Dto;
using DD.ServiceAuth.Domain.Services;
using DD.Shared.Details.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DD.ServiceAuth.Details.Web.Controllers;

[ApiController]
[Route("api/auth/[controller]")]
public class AccountController(
    IUserAuth userAuth,
    IAuthService authService,
    IMapper mapper)
    : ControllerBase
{
    [AllowAnonymous]
    [HttpPost(nameof(SignUp))]
    public Task<SignUpResultDto> SignUp(SignUpInfoDto signUpInfo)
    {
        return authService.SignUpAsync(signUpInfo);
    }

    [AllowAnonymous]
    [HttpPost(nameof(SignIn))]
    public Task<SignInResultDto> SignIn(SignInInfoDto signInInfo)
    {
        return authService.SignInAsync(signInInfo);
    }

    [AllowAnonymous]
    [HttpGet]
    public CurrentUserDto Current()
    {
        var currentUser = userAuth.IsAuthenticated()
            ? mapper.Map<CurrentUserDto>(userAuth.AuthToken())
            : new CurrentUserDto();

        return currentUser;
    }

    [HttpPost(nameof(Renew))]
    public string Renew()
    {
        return authService.RenewToken(userAuth.AuthToken());
    }
}
