using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.Common.Web
{
    public static class StartupExtensions
    {
        public static void AddDarkDeedsTestControllers(this IServiceCollection services)
        {
            services.AddScoped<TestAttribute>();
        }
    }
}