using System.Threading.Tasks;
using DarkDeeds.ServiceAuth.Services.Dto;

namespace DarkDeeds.ServiceAuth.Services.Services.Interface
{
    public interface IAuthService
    {
        Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo);
        Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo);
	}
}