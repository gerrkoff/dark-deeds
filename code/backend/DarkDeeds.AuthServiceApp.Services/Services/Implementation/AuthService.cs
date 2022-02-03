using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Authentication.Core.Models;
using DarkDeeds.Authentication.Core.Services;
using DarkDeeds.AuthServiceApp.Entities;
using DarkDeeds.AuthServiceApp.Services.Dto;
using DarkDeeds.AuthServiceApp.Services.Enums;
using DarkDeeds.AuthServiceApp.Services.Services.Interface;
using DarkDeeds.Common.Validation.Services;
using Microsoft.AspNetCore.Identity;

namespace DarkDeeds.AuthServiceApp.Services.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IValidatorService _validatorService;

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

        public AuthService(UserManager<UserEntity> userManager, ITokenService tokenService, IValidatorService validatorService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _validatorService = validatorService;
        }

        public async Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo)
        {
            _validatorService.Validate(signUpInfo);
            
            var result = new SignUpResultDto();

            var user = new UserEntity { UserName = signUpInfo.Username, DisplayName = signUpInfo.Username };
            IdentityResult createUserResult = await _userManager.CreateAsync(user, signUpInfo.Password);

            if (createUserResult.Succeeded)
            {
                result.Result = SignUpResultEnum.Success;
                result.Token = _tokenService.Serialize(ToAuthToken(user));
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
            _validatorService.Validate(signInInfo);
            
            var result = new SignInResultDto();

            UserEntity user = await _userManager.FindByNameAsync(signInInfo.Username);

            if (user == null)
            {
                result.Result = SignInResultEnum.WrongUsernamePassword;
            }
            else if (!await _userManager.CheckPasswordAsync(user, signInInfo.Password))
            {
                result.Result = SignInResultEnum.WrongUsernamePassword;
            }
            else
            {
                result.Result = SignInResultEnum.Success;
                result.Token = _tokenService.Serialize(ToAuthToken(user));
            }

            return result;
        }

        private AuthToken ToAuthToken(UserEntity user) => new()
        {
            UserId = user.Id,
            Username = user.UserName,
            DisplayName = user.DisplayName,
        };
    }
}