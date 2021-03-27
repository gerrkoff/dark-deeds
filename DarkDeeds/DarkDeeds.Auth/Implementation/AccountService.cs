using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Auth.Dto;
using DarkDeeds.Auth.Enums;
using DarkDeeds.Auth.Interface;
using DarkDeeds.Authentication.Models;
using DarkDeeds.Authentication.Services;
using DarkDeeds.Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace DarkDeeds.Auth.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ITokenService _tokenService;

        #region Identity errors
        private static readonly IdentityErrorDescriber ErrorDescriber = new IdentityErrorDescriber();
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

        public AccountService(UserManager<UserEntity> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<SignUpResultDto> SignUp(SignUpInfoDto signUpInfo)
        {
            var result = new SignUpResultDto();

            var user = new UserEntity { UserName = signUpInfo.Username, DisplayName = signUpInfo.Username };
            IdentityResult createUserResult = await _userManager.CreateAsync(user, signUpInfo.Password);

            if (createUserResult.Succeeded)
            {
                result.Result = SignUpResultEnum.Success;
                result.Token = _tokenService.GetToken(ToCurrentUser(user));
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

        public async Task<SignInResultDto> SignIn(SignInInfoDto signInInfo)
        {
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
                result.Token = _tokenService.GetToken(ToCurrentUser(user));
            }

            return result;
        }

        private CurrentUser ToCurrentUser(UserEntity user) => new CurrentUser()
        {
            UserId = user.Id,
            Username = user.UserName,
            DisplayName = user.DisplayName,
        };
    }
}