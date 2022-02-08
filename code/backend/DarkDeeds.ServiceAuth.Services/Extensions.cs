using DarkDeeds.Common.Validation;
using DarkDeeds.ServiceAuth.Services.Services.Implementation;
using DarkDeeds.ServiceAuth.Services.Services.Interface;
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