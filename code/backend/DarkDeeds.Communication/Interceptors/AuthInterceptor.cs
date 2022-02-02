using System.Threading.Tasks;
using DarkDeeds.Authentication.Core.Services;
using Grpc.Core.Interceptors;

namespace DarkDeeds.Communication.Interceptors
{
    public class AuthInterceptor : ClientAsyncInterceptor
    {
        private readonly IAuthTokenProvider _authTokenProvider;

        public AuthInterceptor(IAuthTokenProvider authTokenProvider)
        {
            _authTokenProvider = authTokenProvider;
        }

        protected override async Task Intercept<TRequest, TResponse>(TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class where TResponse : class
        {
            var token = await _authTokenProvider.GetToken();

            if (token == null)
                return;
            
            context.Options.Headers?.AddAuthorizationIfEmpty(token);
        }
    }
}