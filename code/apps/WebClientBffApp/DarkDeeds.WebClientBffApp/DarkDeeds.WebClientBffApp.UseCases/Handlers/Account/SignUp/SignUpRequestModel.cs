using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.Dto;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.SignUp
{
    public class SignUpRequestModel : IRequest<SignUpResultDto>
    {
        public SignUpRequestModel(SignUpInfoDto signUpInfo)
        {
            SignUpInfo = signUpInfo;
        }

        public SignUpInfoDto SignUpInfo { get; }
    }
}