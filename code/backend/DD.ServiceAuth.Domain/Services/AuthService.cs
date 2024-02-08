using DD.ServiceAuth.Domain.Dto;
using DD.ServiceAuth.Domain.Entities;
using DD.ServiceAuth.Domain.Enums;
using DD.Shared.Auth;
using Microsoft.AspNetCore.Identity;

namespace DD.ServiceAuth.Domain.Services;

public interface IAuthService
{
    Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo);
    Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo);
    Task<string> GetUserIdAsync(string username);
}

class AuthService(
    UserManager<UserEntity> userManager,
    ITokenService tokenService)
    : IAuthService
{
    #region Identity errors
    private static readonly IdentityErrorDescriber ErrorDescriber = new();
    private static readonly string InvalidUsernameCode = ErrorDescriber.InvalidUserName(string.Empty).Code;
    private static readonly string DuplicateUserNameCode = ErrorDescriber.DuplicateUserName(string.Empty).Code;
    private static readonly string[] PasswordErrorCodes = {
        ErrorDescriber.PasswordRequiresDigit().Code,
        ErrorDescriber.PasswordRequiresUpper().Code,
        ErrorDescriber.PasswordRequiresLower().Code,
        ErrorDescriber.PasswordRequiresNonAlphanumeric().Code,
        ErrorDescriber.PasswordTooShort(0).Code,
        ErrorDescriber.PasswordRequiresUniqueChars(0).Code
    };

    #endregion

    public async Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo)
    {
        var user = new UserEntity { UserName = signUpInfo.Username, DisplayName = signUpInfo.Username };
        IdentityResult createUserResult = await userManager.CreateAsync(user, signUpInfo.Password);

        if (createUserResult.Succeeded)
            return new SignUpResultDto
            {
                Result = SignUpResultEnum.Success,
                Token = tokenService.Serialize(ToAuthToken(user))
            };

        if (createUserResult.Errors.Any(x => string.Equals(x.Code, DuplicateUserNameCode)))
            return new SignUpResultDto { Result = SignUpResultEnum.UsernameAlreadyExists };

        if (createUserResult.Errors.Any(x => string.Equals(x.Code, InvalidUsernameCode)))
            return new SignUpResultDto { Result = SignUpResultEnum.InvalidUsername };

        if (createUserResult.Errors.Any(x => PasswordErrorCodes.Contains(x.Code)))
            return new SignUpResultDto { Result = SignUpResultEnum.PasswordInsecure };

        return new SignUpResultDto{ Result = SignUpResultEnum.Unknown };
    }

    public async Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo)
    {
        var user = await userManager.FindByNameAsync(signInInfo.Username);

        if (user == null)
        {
            return new SignInResultDto { Result = SignInResultEnum.WrongUsernamePassword };
        }

        if (!await userManager.CheckPasswordAsync(user, signInInfo.Password))
        {
            return new SignInResultDto { Result = SignInResultEnum.WrongUsernamePassword };
        }

        return new SignInResultDto
        {
            Result = SignInResultEnum.Success,
            Token = tokenService.Serialize(ToAuthToken(user))
        };
    }

    public async Task<string> GetUserIdAsync(string username)
    {
        var user = await userManager.FindByNameAsync(username);

        if (user == null)
            throw new InvalidOperationException();

        return user.Id;
    }

    private static AuthToken ToAuthToken(UserEntity user) => new()
    {
        UserId = user.Id,
        Username = user.UserName ?? string.Empty,
        DisplayName = user.DisplayName ?? string.Empty,
    };
}
