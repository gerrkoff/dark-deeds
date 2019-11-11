using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DarkDeeds.Data.Context
{
    public class DarkDeedsContextDesignTimeFactory : IDesignTimeDbContextFactory<DarkDeedsContext>
    {
        public DarkDeedsContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.Development.json")
                .Build();
            string connectionString = configuration.GetConnectionString("appDb");
            var optionsBuilder = new DbContextOptionsBuilder<DarkDeedsContext>();
            optionsBuilder.UseNpgsql(connectionString);
            
            return new DarkDeedsContext(optionsBuilder.Options);
        }
    }
}