using DarkDeeds.Common.Validation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.Common.Validation
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDarkDeedsValidation(this IServiceCollection services)
        {
            services.AddScoped<IValidatorService, ValidatorService>();
        }
    }
}