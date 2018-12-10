using System;
using System.IO.Compression;
using System.Text;
using AutoMapper;
using DarkDeeds.Api.Filters;
using DarkDeeds.AutoMapper;
using DarkDeeds.Common.Settings;
using DarkDeeds.Data.Context;
using DarkDeeds.Data.Entity;
using DarkDeeds.Data.Repository;
using DarkDeeds.Services.Implementation;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DarkDeeds.Api
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            IdentityBuilder builder = services.AddIdentityCore<UserEntity>(options =>
            {
                options.Password.RequiredLength = 8;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<DarkDeedsContext>();
            
            services.AddScoped<UserManager<UserEntity>>();
            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
        
        public static IServiceCollection ConfigureAutomapper(this IServiceCollection services)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<MappingProfile>());
            return services;
        }
        
        public static IServiceCollection ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthSettings>(options => configuration.GetSection("Auth").Bind(options));
            return services;
        }
        
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services)
        {
            // TODO: move to settings
            const string constring = "Server=localhost,1433;Database=darkdeeds;User=sa;Password=Password1";
            services.AddDbContext<DarkDeedsContext>(options => options.UseSqlServer(constring));
            services.AddScoped<DbContext, DarkDeedsContext>();
            return services;
        }

        public static IServiceCollection ConfigureMvc(this IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.Filters.Add(typeof(ExceptionHandlerFilter));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            return services;
        }

        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            AuthSettings authSettings = configuration.GetSection("Auth").Get<AuthSettings>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = authSettings.Audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings.Key)),
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }
        
        public static IServiceCollection AddCompression(this IServiceCollection services)
        {
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.MimeTypes = new[]
                {
                    // Default
                    "text/plain",
                    "text/css",
                    "application/javascript",
                    "text/html",
                    // "application/xml",
                    // "text/xml",
                    // "application/json",
                    // "text/json",

                    // Custom
                    "image/svg+xml"
                };
            });

            return services;
        }
    }
}