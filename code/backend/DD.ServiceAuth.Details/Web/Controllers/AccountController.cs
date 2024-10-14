using AutoMapper;
using DD.ServiceAuth.Domain.Dto;
using DD.ServiceAuth.Domain.Services;
using DD.Shared.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DD.ServiceAuth.Details.Web.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/auth/[controller]")]
public class AccountController(
    IUserAuth userAuth,
    IAuthService authService,
    IMapper mapper)
    : ControllerBase
{
    [HttpPost(nameof(SignUp))]
    public Task<SignUpResultDto> SignUp(SignUpInfoDto signUpInfo)
    {
        return authService.SignUpAsync(signUpInfo);
    }

    [HttpPost(nameof(SignIn))]
    public async Task<SignInResultDto> SignIn(SignInInfoDto signInInfo)
    {
        await Task.Delay(4000);
        return await authService.SignInAsync(signInInfo);
    }

    [HttpGet]
    public async Task<CurrentUserDto> Current()
    {
        await Task.Delay(100);
        var currentUser = userAuth.IsAuthenticated()
            ? mapper.Map<CurrentUserDto>(userAuth.AuthToken())
            : new CurrentUserDto();

        return currentUser;
    }
}
