using System.Threading.Tasks;
using DarkDeeds.WebClientBff.Infrastructure.Communication.AuthServiceApp.Dto;

namespace DarkDeeds.WebClientBff.Infrastructure.Communication.AuthServiceApp
{
    public interface IAuthServiceApp
    {
        Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo);
        Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo);
    }
}