using System.Threading.Tasks;
using DarkDeeds.Auth.Dto;

namespace DarkDeeds.Auth.Interface
{
    public interface IAccountService
    {
        Task<SignUpResultDto> SignUp(SignUpInfoDto signUpInfo);
        Task<SignInResultDto> SignIn(SignInInfoDto signInInfo);
	}
}