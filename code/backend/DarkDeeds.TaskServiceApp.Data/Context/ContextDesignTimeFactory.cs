using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DarkDeeds.TaskServiceApp.Data.Context
{
    public class ContextDesignTimeFactory : IDesignTimeDbContextFactory<DarkDeedsTaskContext>
    {
        public DarkDeedsTaskContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.Development.json")
                .Build();
            string connectionString = configuration.GetConnectionString("appDb");
            var optionsBuilder = new DbContextOptionsBuilder<DarkDeedsTaskContext>();
            optionsBuilder.UseNpgsql(connectionString);
            
            return new DarkDeedsTaskContext(optionsBuilder.Options);
        }
    }
}