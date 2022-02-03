using DarkDeeds.ServiceAuth.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.ServiceAuth.Data
{
    public static class DependencyInjectionExtensions
    {
        public static void AddAuthServiceDatabase(this IServiceCollection services, IConfiguration configuration)
        {            
            string connectionString = configuration.GetConnectionString("appDb");
            services.AddDbContext<DarkDeedsAuthContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<DbContext, DarkDeedsAuthContext>();
        }
    }
}
