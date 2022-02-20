using System.Threading.Tasks;
using DarkDeeds.ServiceAuth.Dto.Dto;

namespace DarkDeeds.ServiceAuth.Services
{
    public interface IAuthService
    {
        Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo);
        Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo);
	}
}
