using System;
using Microsoft.Extensions.Configuration;

namespace DarkDeeds.MongoMigrator.Helpers
{
    public class ConfigFactory
    {
        public IConfiguration Get()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("Config/appsettings.Production.json")
                .Build();
        }
    }
}