using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace DarkDeeds.Communication.Interceptors
{
    public class AuthInterceptor : Interceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var token = _httpContextAccessor.HttpContext.Items["access_token"] as string;

            if (!string.IsNullOrEmpty(token))
            {
                context.Options.Headers.Add(HeaderNames.Authorization, $"Bearer {token}");
            }
            
            return continuation(request, context);
        }
    }
}