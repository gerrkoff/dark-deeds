using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.CommonWeb
{
    public static class StartupExtensions
    {
        public static void AddDarkDeedsTestControllers(this IServiceCollection services)
        {
            services.AddScoped<TestAttribute>();
        }
    }
}