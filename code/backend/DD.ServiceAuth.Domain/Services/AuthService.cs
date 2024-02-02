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
        var result = new SignUpResultDto();

        var user = new UserEntity { UserName = signUpInfo.Username, DisplayName = signUpInfo.Username };
        IdentityResult createUserResult = await userManager.CreateAsync(user, signUpInfo.Password);

        if (createUserResult.Succeeded)
        {
            result.Result = SignUpResultEnum.Success;
            result.Token = tokenService.Serialize(ToAuthToken(user));
        }
        else if (createUserResult.Errors.Any(x => string.Equals(x.Code, DuplicateUserNameCode)))
        {
            result.Result = SignUpResultEnum.UsernameAlreadyExists;
        }
        else if (createUserResult.Errors.Any(x => string.Equals(x.Code, InvalidUsernameCode)))
        {
            result.Result = SignUpResultEnum.InvalidUsername;
        }
        else if (createUserResult.Errors.Any(x => PasswordErrorCodes.Contains(x.Code)))
        {
            result.Result = SignUpResultEnum.PasswordInsecure;
        }
        else
        {
            result.Result = SignUpResultEnum.Unknown;
        }

        return result;
    }

    public async Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo)
    {
        var result = new SignInResultDto();

        UserEntity user = await userManager.FindByNameAsync(signInInfo.Username);

        if (user == null)
        {
            result.Result = SignInResultEnum.WrongUsernamePassword;
        }
        else if (!await userManager.CheckPasswordAsync(user, signInInfo.Password))
        {
            result.Result = SignInResultEnum.WrongUsernamePassword;
        }
        else
        {
            result.Result = SignInResultEnum.Success;
            result.Token = tokenService.Serialize(ToAuthToken(user));
        }

        return result;
    }

    public async Task<string> GetUserIdAsync(string username)
    {
        var user = await userManager.FindByNameAsync(username);

        if (user == null)
            throw new InvalidOperationException();

        return user.Id;
    }

    private AuthToken ToAuthToken(UserEntity user) => new()
    {
        UserId = user.Id,
        Username = user.UserName,
        DisplayName = user.DisplayName,
    };
}
