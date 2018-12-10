using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.Data.Entity;
using DarkDeeds.Enums;
using DarkDeeds.Models.Account;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace DarkDeeds.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ITokenService _tokenService;

        public AccountService(UserManager<UserEntity> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<RegisterResultDto> Register(RegisterInfoDto registerInfo)
        {
            var result = new RegisterResultDto();

            var user = new UserEntity { UserName = registerInfo.Username, DisplayName = registerInfo.Username };
            IdentityResult createUserResult = await _userManager.CreateAsync(user, registerInfo.Password);

            if (createUserResult.Succeeded)
            {
                result.Result = RegisterResultEnum.Success;
                result.Token = _tokenService.GetToken(user);
            }
            else if (createUserResult.Errors.Any(x => string.Equals(x.Code, "DuplicateUserName")))
            {
                result.Result = RegisterResultEnum.UsernameAlreadyExists;
            }
            // TODO: password is too simple case
            else
            {
                result.Result = RegisterResultEnum.Unknown;
            }

            return result;
        }

        public async Task<LoginResultDto> Login(LoginInfoDto loginInfo)
        {
            var result = new LoginResultDto();

            UserEntity user = await _userManager.FindByNameAsync(loginInfo.Username);

            if (user == null)
            {
                result.Result = LoginResultEnum.WrongUsernamePassword;
            }
            else if (!await _userManager.CheckPasswordAsync(user, loginInfo.Password))
            {
                result.Result = LoginResultEnum.WrongUsernamePassword;
            }
            else
            {
                result.Result = LoginResultEnum.Success;
                result.Token = _tokenService.GetToken(user);
            }

            return result;
        }
	}
}