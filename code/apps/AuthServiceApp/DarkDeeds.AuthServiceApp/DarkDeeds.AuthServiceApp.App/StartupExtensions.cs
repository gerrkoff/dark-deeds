using DarkDeeds.AuthServiceApp.Data.Context;
using DarkDeeds.AuthServiceApp.Entities;
using DarkDeeds.AuthServiceApp.Services.Implementation;
using DarkDeeds.AuthServiceApp.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.AuthServiceApp.App
{
    public static class StartupExtensions
    {
        public static void AddAuthIdentity(this IServiceCollection services)
        {
            IdentityBuilder builder = services.AddIdentityCore<UserEntity>(options =>
            {
#if DEBUG
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
#endif
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<DarkDeedsAuthContext>();
            
            services.AddScoped<UserManager<UserEntity>>();
        }

        public static void AddAuthServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
        }

        public static void AddAuthDatabase(this IServiceCollection services, IConfiguration configuration)
        {            
            string connectionString = configuration.GetConnectionString("appDb");
            services.AddDbContext<DarkDeedsAuthContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<DbContext, DarkDeedsAuthContext>();
        }
    }
}