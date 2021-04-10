using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.Authentication.DependencyInjection.Middlewares
{
    public class ParseAuthTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public ParseAuthTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = await context.GetTokenAsync("access_token");
            context.Items.Add("access_token", token);

            await _next(context);
        }
    }
}