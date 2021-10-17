using DarkDeeds.AuthServiceApp.Services.Services.Implementation;
using DarkDeeds.AuthServiceApp.Services.Services.Interface;
using DarkDeeds.Common.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.AuthServiceApp.Services
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