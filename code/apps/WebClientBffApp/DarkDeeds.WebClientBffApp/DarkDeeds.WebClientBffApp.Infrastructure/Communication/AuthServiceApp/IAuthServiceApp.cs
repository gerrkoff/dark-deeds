using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto;

namespace DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp
{
    public interface IAuthServiceApp
    {
        Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo);
        Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo);
    }
}