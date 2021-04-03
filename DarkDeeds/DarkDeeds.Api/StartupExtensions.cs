using System.IO.Compression;
using AutoMapper;
using DarkDeeds.Api.Filters;
using DarkDeeds.Api.InfrastructureServices;
using DarkDeeds.Communication;
using DarkDeeds.Data.Context;
using DarkDeeds.Data.Repository;
using DarkDeeds.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.Infrastructure.Communication.TaskServiceApp;
using DarkDeeds.Infrastructure.Communication.TelegramClientApp;
using DarkDeeds.Infrastructure.Data;
using DarkDeeds.Infrastructure.Services;
using DarkDeeds.Models.Mapping;
using DarkDeeds.Services.Implementation;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace DarkDeeds.Api
{
    public static class StartupExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepositoryNonDeletable<>), typeof(RepositoryNonDeletable<>));
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<ITaskHubService, TaskHubService>();
            
            services.AddScoped<ITaskServiceApp, TaskServiceApp>();
            services.AddScoped<IAuthServiceApp, AuthServiceApp>();
            services.AddScoped<ITelegramClientApp, TelegramClientApp>();

            return services;
        }
        
        public static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ModelsMappingProfile>();
            });
            return services;
        }

        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {            
            string connectionString = configuration.GetConnectionString("appDb");
            services.AddDbContext<DarkDeedsContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<DbContext, DarkDeedsContext>();
            return services;
        }

        public static IServiceCollection ConfigureMvc(this IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.Filters.Add(typeof(ExceptionHandlerFilter));
                    
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
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