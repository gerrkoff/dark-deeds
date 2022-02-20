using DarkDeeds.Common.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.ServiceAuth.Services
{
    public static class Extensions
    {   
        public static void AddAuthServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddDarkDeedsValidation();
        }
    }
}