using DarkDeeds.Common.Validation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.Common.Validation
{
    public static class DiExtensions
    {
        public static void AddDarkDeedsValidation(this IServiceCollection services)
        {
            services.AddScoped<IValidatorService, ValidatorService>();
        }
    }
}