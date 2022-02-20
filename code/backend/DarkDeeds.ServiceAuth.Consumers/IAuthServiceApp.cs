using System.Threading.Tasks;
using DarkDeeds.ServiceAuth.Dto.Dto;

namespace DarkDeeds.ServiceAuth.Consumers
{
    public interface IAuthServiceApp
    {
        Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo);
        Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo);
    }
}
