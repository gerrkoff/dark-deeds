using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DarkDeeds.Data.Context
{
    public class DarkDeedsContextDesignTimeFactory : IDesignTimeDbContextFactory<DarkDeedsContext>
    {
        public DarkDeedsContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DarkDeedsContext>();
            const string constring = "Server=localhost,1433;Database=darkdeeds;User=sa;Password=<YourStrong!Passw0rd>";
            optionsBuilder.UseSqlServer(constring);
            
            return new DarkDeedsContext(optionsBuilder.Options);
        }
    }
}