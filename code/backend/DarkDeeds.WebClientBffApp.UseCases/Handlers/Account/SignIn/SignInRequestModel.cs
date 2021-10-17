using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.SignIn
{
    public class SignInRequestModel : IRequest<SignInResultDto>
    {
        public SignInRequestModel(SignInInfoDto signInInfo)
        {
            SignInInfo = signInInfo;
        }

        public SignInInfoDto SignInInfo { get; }
    }
}