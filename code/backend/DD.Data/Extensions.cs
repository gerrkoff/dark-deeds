using DarkDeeds.Backend.Data;
using DarkDeeds.Backend.Entities;
using DarkDeeds.Backend.Entities.Entity;
using DD.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DD.Data;

public static class Extensions
{
    public static void AddBackendDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("appDb");

        services.AddDbContext<BackendDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<DbContext, BackendDbContext>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddIdentityCore<UserEntity>(options =>
            {
#if DEBUG
                options.Password.RequiredLength = 3;
#else
                    options.Password.RequiredLength = 8;
#endif
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<BackendDbContext>()
            .AddUserManager<UserManager<UserEntity>>();
    }
}
