using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.ServiceAuth.Consumers;
using DarkDeeds.ServiceAuth.Dto.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Account.SignUp
{
    class SignUpRequestHandler : IRequestHandler<SignUpRequestModel, SignUpResultDto>
    {
        private readonly IAuthServiceApp _authServiceApp;

        public SignUpRequestHandler(IAuthServiceApp authServiceApp)
        {
            _authServiceApp = authServiceApp;
        }

        public Task<SignUpResultDto> Handle(SignUpRequestModel request, CancellationToken cancellationToken)
        {
            return _authServiceApp.SignUpAsync(request.SignUpInfo);
        }
    }
}
