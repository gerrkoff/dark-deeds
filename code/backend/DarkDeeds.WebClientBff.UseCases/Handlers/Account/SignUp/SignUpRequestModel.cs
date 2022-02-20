using DarkDeeds.ServiceAuth.Dto.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Account.SignUp
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
