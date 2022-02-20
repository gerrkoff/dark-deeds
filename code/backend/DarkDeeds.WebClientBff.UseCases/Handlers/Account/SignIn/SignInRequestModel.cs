using DarkDeeds.ServiceAuth.Dto.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Account.SignIn
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
