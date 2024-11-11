using DD.ServiceAuth.Domain.Dto;
using DD.ServiceAuth.Domain.Entities;
using DD.ServiceAuth.Domain.Enums;
using DD.Shared.Details.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using SignInResult = DD.ServiceAuth.Domain.Enums.SignInResult;

namespace DD.ServiceAuth.Domain.Services;

public interface IAuthService
{
    Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo);

    Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo);

    Task<string> GetUserIdAsync(string username);

    string RenewToken(AuthToken authToken);
}

internal sealed class AuthService(
    UserManager<UserEntity> userManager,
    ITokenService tokenService)
    : IAuthService
{
    private static readonly IdentityErrorDescriber ErrorDescriber = new();
    private static readonly string InvalidUsernameCode = ErrorDescriber.InvalidUserName(string.Empty).Code;
    private static readonly string DuplicateUserNameCode = ErrorDescriber.DuplicateUserName(string.Empty).Code;
    private static readonly string[] PasswordErrorCodes =
    [
        ErrorDescriber.PasswordRequiresDigit().Code,
        ErrorDescriber.PasswordRequiresUpper().Code,
        ErrorDescriber.PasswordRequiresLower().Code,
        ErrorDescriber.PasswordRequiresNonAlphanumeric().Code,
        ErrorDescriber.PasswordTooShort(0).Code,
        ErrorDescriber.PasswordRequiresUniqueChars(0).Code
    ];

    public async Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo)
    {
        var user = new UserEntity { UserName = signUpInfo.Username, DisplayName = signUpInfo.Username };
        var createUserResult = await userManager.CreateAsync(user, signUpInfo.Password);

        if (createUserResult.Succeeded)
        {
            return new SignUpResultDto
            {
                Result = SignUpResult.Success,
                Token = tokenService.Serialize(ToAuthToken(user)),
            };
        }

        if (createUserResult.Errors.Any(x => string.Equals(x.Code, DuplicateUserNameCode, StringComparison.Ordinal)))
            return new SignUpResultDto { Result = SignUpResult.UsernameAlreadyExists };

        if (createUserResult.Errors.Any(x => string.Equals(x.Code, InvalidUsernameCode, StringComparison.Ordinal)))
            return new SignUpResultDto { Result = SignUpResult.InvalidUsername };

        if (createUserResult.Errors.Any(x => PasswordErrorCodes.Contains(x.Code)))
            return new SignUpResultDto { Result = SignUpResult.PasswordInsecure };

        return new SignUpResultDto { Result = SignUpResult.Unknown };
    }

    public async Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo)
    {
        var user = await userManager.FindByNameAsync(signInInfo.Username);

        if (user == null)
        {
            return new SignInResultDto { Result = SignInResult.WrongUsernamePassword };
        }

        if (!await userManager.CheckPasswordAsync(user, signInInfo.Password))
        {
            return new SignInResultDto { Result = SignInResult.WrongUsernamePassword };
        }

        return new SignInResultDto
        {
            Result = SignInResult.Success,
            Token = tokenService.Serialize(ToAuthToken(user)),
        };
    }

    public async Task<string> GetUserIdAsync(string username)
    {
        var user = await userManager.FindByNameAsync(username)
                   ?? throw new InvalidOperationException();

        return user.Id.ToString();
    }

    public string RenewToken(AuthToken authToken)
    {
        return tokenService.Serialize(authToken);
    }

    private static AuthTokenBuildInfo ToAuthToken(UserEntity user)
    {
        return new AuthTokenBuildInfo
        {
            UserId = user.Id.ToString(),
            Username = user.UserName ?? throw new InvalidOperationException("Username is null"),
            DisplayName = user.DisplayName,
        };
    }
}
