using DarkDeeds.Authentication.Services;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

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
            FindAndSetAuthToken(context);
            
            return continuation(request, context);
        }

        private void FindAndSetAuthToken<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            if (_httpContextAccessor.HttpContext == null) return;

            var token = _httpContextAccessor.HttpContext.Items["access_token"] as string;

            if (!string.IsNullOrWhiteSpace(token))
                context.Options.Headers.AddAuthorization(token);
        }
    }
}