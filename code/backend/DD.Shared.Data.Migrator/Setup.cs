using System.Reflection;
using DD.Shared.Data.Migrator.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DD.Shared.Data.Migrator;

public static class Setup
{
    public static void AddSharedDataMigrator(this IServiceCollection services)
    {
        services.AddMigrations();
        services.AddScoped<MigrationProvider>();
        services.AddScoped<MigrationApplier>();
        services.AddHostedService<MigrationRunner>();
    }

    private static void AddMigrations(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var type in assembly.GetTypes().OrderBy(x => x.Name))
        {
            if (type.GetInterfaces().Contains(typeof(IMigrationBody)))
            {
                services.AddScoped(typeof(IMigrationBody), type);
            }
        }
    }
}
