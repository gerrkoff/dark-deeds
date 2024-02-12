using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DD.Shared.Data.Context;

internal sealed class DarkDeedsContextDesignTimeFactory : IDesignTimeDbContextFactory<BackendDbContext>
{
    public BackendDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("design-time-connection.json")
            .Build();
        var connectionString = configuration.GetConnectionString("sharedDb");
        var optionsBuilder = new DbContextOptionsBuilder<BackendDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new BackendDbContext(optionsBuilder.Options);
    }
}
