using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DarkDeeds.MongoMigrator.PostgreDal.Context
{
    public class ContextDesignTimeFactory
    {
        public DarkDeedsTaskContext CreateDbContext(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("pgDb");
            var optionsBuilder = new DbContextOptionsBuilder<DarkDeedsTaskContext>();
            optionsBuilder.UseNpgsql(connectionString);
            
            return new DarkDeedsTaskContext(optionsBuilder.Options);
        }
    }
}