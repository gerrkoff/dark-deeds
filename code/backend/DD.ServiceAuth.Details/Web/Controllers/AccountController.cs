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
    IValidator validator,
    IAuthService authService,
    IMapper mapper)
    : ControllerBase
{
    [HttpPost(nameof(SignUp))]
    public Task<SignUpResultDto> SignUp(SignUpInfoDto signUpInfo)
    {
        validator.Validate(ModelState);
        return authService.SignUpAsync(signUpInfo);
    }

    [HttpPost(nameof(SignIn))]
    public Task<SignInResultDto> SignIn(SignInInfoDto signInInfo)
    {
        validator.Validate(ModelState);
        return authService.SignInAsync(signInInfo);
    }

    [HttpGet]
    public Task<CurrentUserDto> Current()
    {
        var currentUser = userAuth.IsAuthenticated()
            ? mapper.Map<CurrentUserDto>(userAuth.AuthToken())
            : new CurrentUserDto();

        return Task.FromResult(currentUser);
    }
}
