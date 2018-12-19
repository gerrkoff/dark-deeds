using System.Threading.Tasks;
using DarkDeeds.Models.Account;

namespace DarkDeeds.Services.Interface
{
    public interface IAccountService
    {
        Task<SignUpResultDto> SignUp(SignUpInfoDto signUpInfo);
        Task<SignInResultDto> SignIn(SignInInfoDto signInInfo);
	}
}