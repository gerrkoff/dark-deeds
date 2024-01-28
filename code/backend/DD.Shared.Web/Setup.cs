using Microsoft.Extensions.DependencyInjection;

namespace DD.Shared.Web;

public static class Setup
{
    public static void AddDdSharedWeb(this IServiceCollection services)
    {
        services.AddScoped<TestAttribute>();
        services.AddScoped<IUserAuth, UserAuth>();
        services.AddScoped<IValidator, Validator>();
    }
}
