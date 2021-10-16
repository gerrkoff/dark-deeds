using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DarkDeeds.AuthServiceApp.Data.Context
{
    public class DarkDeedsContextDesignTimeFactory : IDesignTimeDbContextFactory<DarkDeedsAuthContext>
    {
        public DarkDeedsAuthContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.Development.json")
                .Build();
            string connectionString = configuration.GetConnectionString("appDb");
            var optionsBuilder = new DbContextOptionsBuilder<DarkDeedsAuthContext>();
            optionsBuilder.UseNpgsql(connectionString);
            
            return new DarkDeedsAuthContext(optionsBuilder.Options);
        }
    }
}
