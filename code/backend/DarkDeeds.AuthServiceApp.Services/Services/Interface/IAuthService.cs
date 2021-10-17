using System.Threading.Tasks;
using DarkDeeds.AuthServiceApp.Services.Dto;

namespace DarkDeeds.AuthServiceApp.Services.Services.Interface
{
    public interface IAuthService
    {
        Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo);
        Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo);
	}
}