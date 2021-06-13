using DarkDeeds.Authentication.Models;
using DarkDeeds.Authentication.Services;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace DarkDeeds.Communication.Interceptors
{
    public class AuthInterceptor : Interceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;

        public AuthInterceptor(IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            if (!TryGetToken(context))
                TryGetUserId(context);
            
            return continuation(request, context);
        }

        private bool TryGetToken<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var token = _httpContextAccessor.HttpContext.Items["access_token"] as string;

            if (string.IsNullOrWhiteSpace(token))
                return false;

            AddAuthHeader(context, token);
            return true;
        }
        
        private bool TryGetUserId<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var userId = context.Options.Headers.Get("UserId");

            if (string.IsNullOrWhiteSpace(userId.Value))
                return false;

            var token = _tokenService.Serialize(new AuthToken {UserId = userId.Value});
            AddAuthHeader(context, token);
            return true;
        }

        private void AddAuthHeader<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, string token)
            where TRequest : class 
            where TResponse : class
        {
            context.Options.Headers.Add(HeaderNames.Authorization, $"Bearer {token}");
        }
    }
}