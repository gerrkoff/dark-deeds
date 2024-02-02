using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DD.Shared.Data.Context;

class DarkDeedsContextDesignTimeFactory : IDesignTimeDbContextFactory<BackendDbContext>
{
    public BackendDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("design-time-connection.json")
            .Build();
        string connectionString = configuration.GetConnectionString("sharedDb");
        var optionsBuilder = new DbContextOptionsBuilder<BackendDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new BackendDbContext(optionsBuilder.Options);
    }
}
