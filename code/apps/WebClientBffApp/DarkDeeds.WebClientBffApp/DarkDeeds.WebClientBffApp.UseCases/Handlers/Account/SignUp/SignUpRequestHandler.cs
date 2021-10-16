using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.SignUp
{
    public class SignUpRequestHandler : IRequestHandler<SignUpRequestModel, SignUpResultDto>
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