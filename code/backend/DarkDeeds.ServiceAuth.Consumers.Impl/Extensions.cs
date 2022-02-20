using DarkDeeds.Communication;
using DarkDeeds.ServiceAuth.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.ServiceAuth.Consumers.Impl
{
    public static class Extensions
    {
        public static void AddAuthServiceApp(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDarkDeedsGrpcClientFactory<AuthService.AuthServiceClient>("auth-service", configuration);
            services.AddScoped<IAuthServiceApp, AuthServiceApp>();

            services.AddAutoMapper(typeof(ModelsMapping));
        }
    }
}
