using DarkDeeds.WebClientBffApp.Data.Context;
using DarkDeeds.WebClientBffApp.Data.Repository;
using DarkDeeds.WebClientBffApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.WebClientBffApp.Data
{
    public static class DependencyInjectionExtensions
    {
        public static void AddWebClientBffData(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("appDb");
            services.AddDbContext<DarkDeedsWebClientBffContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<DbContext, DarkDeedsWebClientBffContext>();
            
            services.AddScoped(typeof(IRepositoryNonDeletable<>), typeof(RepositoryNonDeletable<>));
        }
    }
}