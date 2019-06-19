using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DarkDeeds.Data.Context
{
    public class DarkDeedsContextDesignTimeFactory : IDesignTimeDbContextFactory<DarkDeedsContext>
    {
        public DarkDeedsContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DarkDeedsContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=darkdeeds;Username=postgres;");
            
            return new DarkDeedsContext(optionsBuilder.Options);
        }
    }
}