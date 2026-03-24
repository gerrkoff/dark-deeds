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
    IAuthService authService)
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
        if (!userAuth.IsAuthenticated())
            return new CurrentUserDto();

        var token = userAuth.AuthToken();
        return new CurrentUserDto
        {
            Username = token.DisplayName,
            UserAuthenticated = !string.IsNullOrEmpty(token.Username),
            Expires = token.Expires,
        };
    }

    [HttpPost(nameof(Renew))]
    public string Renew()
    {
        return authService.RenewToken(userAuth.AuthToken());
    }
}
