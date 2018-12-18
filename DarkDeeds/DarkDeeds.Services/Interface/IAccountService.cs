using System.Threading.Tasks;
using DarkDeeds.Models.Account;

namespace DarkDeeds.Services.Interface
{
    public interface IAccountService
    {
        Task<RegisterResultDto> SignUp(RegisterInfoDto registerInfo);
        Task<LoginResultDto> SignIn(LoginInfoDto loginInfo);
	}
}