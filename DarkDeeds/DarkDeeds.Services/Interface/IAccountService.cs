using System.Threading.Tasks;
using DarkDeeds.Models.Account;

namespace DarkDeeds.Services.Interface
{
    public interface IAccountService
    {
        Task<RegisterResultDto> Register(RegisterInfoDto registerInfo);
        Task<LoginResultDto> Login(LoginInfoDto loginInfo);
	}
}