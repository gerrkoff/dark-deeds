using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.WebClientBff.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.WebClientBff.Infrastructure.Communication.AuthServiceApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBff.UseCases.Handlers.Account.SignIn
{
    class SignInRequestHandler : IRequestHandler<SignInRequestModel, SignInResultDto>
    {
        private readonly IAuthServiceApp _authServiceApp;

        public SignInRequestHandler(IAuthServiceApp authServiceApp)
        {
            _authServiceApp = authServiceApp;
        }

        public Task<SignInResultDto> Handle(SignInRequestModel request, CancellationToken cancellationToken)
        {
            return _authServiceApp.SignInAsync(request.SignInInfo);
        }
    }
}