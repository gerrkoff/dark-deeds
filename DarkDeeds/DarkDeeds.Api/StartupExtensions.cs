using System;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.Api.Filters;
using DarkDeeds.Api.InfrastructureServices;
using DarkDeeds.Auth.Implementation;
using DarkDeeds.Auth.Interface;
using DarkDeeds.Auth.Mapping;
using DarkDeeds.Auth.Settings;
using DarkDeeds.BotIntegration.Implementation;
using DarkDeeds.BotIntegration.Implementation.CommandProcessor;
using DarkDeeds.BotIntegration.Interface;
using DarkDeeds.BotIntegration.Interface.CommandProcessor;
using DarkDeeds.Data.Context;
using DarkDeeds.Data.Repository;
using DarkDeeds.Entities.Models;
using DarkDeeds.Infrastructure.Data;
using DarkDeeds.Infrastructure.Services;
using DarkDeeds.Models.Mapping;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace DarkDeeds.Api
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
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
                .AddEntityFrameworkStores<DarkDeedsContext>();
            
            services.AddScoped<UserManager<UserEntity>>();
            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IRepositoryNonDeletable<>), typeof(RepositoryNonDeletable<>));
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITaskParserService, TaskParserService>();
            services.AddScoped<ITelegramService, TelegramService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IRecurrenceCreatorService, RecurrenceCreatorService>();
            services.AddScoped<IDateService, DateService>();
            services.AddScoped<IRecurrenceService, RecurrenceService>();
            services.AddScoped<IPermissionsService, PermissionsService>();
            services.AddScoped<ITaskHubService, TaskHubService>();
            
            services.AddScoped<IBotCommandParserService, BotCommandParserService>();
            services.AddScoped<IShowTodoCommandProcessor, ShowTodoCommandProcessor>();
            services.AddScoped<ICreateTaskCommandProcessor, CreateTaskCommandProcessor>();
            services.AddScoped<IStartCommandProcessor, StartCommandProcessor>();
            services.AddScoped<IBotProcessMessageService, BotProcessMessageService>();
            services.AddScoped<IBotSendMessageService>(provider => new BotSendMessageService(configuration["Bot"]));
#if DEBUG
            services.AddScoped<IBotSendMessageService>(provider => new BotSendMessageDebugService(configuration["Bot"]));
#endif
            return services;
        }
        
        public static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ModelsMappingProfile>();
                cfg.AddProfile<AuthMappingProfile>();
            });
            return services;
        }
        
        public static IServiceCollection ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthSettings>(options => configuration.GetSection("Auth").Bind(options));
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
                    
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/ws"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
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