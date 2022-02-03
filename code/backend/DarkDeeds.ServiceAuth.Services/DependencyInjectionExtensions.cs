using DarkDeeds.Common.Validation;
using DarkDeeds.ServiceAuth.Services.Services.Implementation;
using DarkDeeds.ServiceAuth.Services.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.ServiceAuth.Services
{
    public static class DependencyInjectionExtensions
    {   
        public static void AddAuthServiceServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddDarkDeedsValidation();
        }
    }
}