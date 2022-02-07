using DarkDeeds.Common.Misc;
using DarkDeeds.Communication.Interceptors;
using DarkDeeds.ServiceTask.Communication;
using DarkDeeds.ServiceTask.Data;
using DarkDeeds.ServiceTask.Data.EntityRepository;
using DarkDeeds.ServiceTask.Infrastructure.Data.EntityRepository;
using DarkDeeds.ServiceTask.Infrastructure.Services;
using DarkDeeds.ServiceTask.Models.Mapping;
using DarkDeeds.ServiceTask.Services.Implementation;
using DarkDeeds.ServiceTask.Services.Interface;
using DarkDeeds.ServiceTask.Services.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DarkDeeds.ServiceTask.App
{
    public static class StartupExtensions
    {
        public static void AddTaskServices(this IServiceCollection services)
        {
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ITaskParserService, TaskParserService>();
            services.AddScoped<IRecurrenceCreatorService, RecurrenceCreatorService>();
            services.AddScoped<IDateService, DateService>();
            services.AddScoped<IRecurrenceService, RecurrenceService>();
            services.AddScoped<INotifierService, NotifierService>();
            services.AddScoped<ITaskSpecification, TaskSpecification>();
            services.AddScoped<IPlannedRecurrenceSpecification, PlannedRecurrenceSpecification>();
            services.AddScoped<ISpecificationFactory, SpecificationFactory>();
        }
        
        public static void AddTaskAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ModelsMappingProfile));
        }

        public static void AddTaskDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("mongoDb");
            services.AddSingleton<IMongoDbContext>(_ => new MongoDbContext(connectionString));
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IPlannedRecurrenceRepository, PlannedRecurrenceRepository>();
        }

        public static void AddTaskServiceApi(this IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.Interceptors.Add<ExceptionInterceptor>();
            });
            services.AddGrpcReflection();
            
            services.AddControllers(options =>
            {
                var authRequired = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(authRequired));
            });

            var buildInfo = new BuildInfo(typeof(Startup), typeof(Contract.ParserService));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DarkDeeds.TaskService", 
                    Version = $"{buildInfo.AppVersion} / {buildInfo.ContractVersion}",
                    Description = "Check gRPC contract",
                });
            });
        }
    }
}