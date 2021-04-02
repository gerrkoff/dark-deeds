using System.Threading.Tasks;
using DarkDeeds.Infrastructure.Communication.AuthServiceApp.Dto;

namespace DarkDeeds.Infrastructure.Communication.AuthServiceApp
{
    public interface IAuthServiceApp
    {
        Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo);
        Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo);
    }
}
