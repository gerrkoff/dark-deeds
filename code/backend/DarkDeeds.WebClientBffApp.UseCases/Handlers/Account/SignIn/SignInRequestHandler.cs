using System.Threading;
using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto;
using MediatR;

namespace DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.SignIn
{
    public class SignInRequestHandler : IRequestHandler<SignInRequestModel, SignInResultDto>
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