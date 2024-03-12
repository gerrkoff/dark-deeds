using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DD.Shared.Data;

public static class Setup
{
    public static void AddSharedData(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("sharedDb")
                                    ?? throw new InvalidOperationException("Connection string for sharedDb is not found");
        services.AddSingleton<IMigratorMongoDbContext>(_ => new MongoDbContext(mongoConnectionString));
        services.AddSingleton<IMongoDbContext>(sp => sp.GetRequiredService<IMigratorMongoDbContext>());
    }
}
