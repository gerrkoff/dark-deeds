using AutoMapper;
using DarkDeeds.TaskServiceApp.Communication;
using DarkDeeds.TaskServiceApp.Data.Context;
using DarkDeeds.TaskServiceApp.Data.Repository;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using DarkDeeds.TaskServiceApp.Infrastructure.Services;
using DarkDeeds.TaskServiceApp.Models.Mapping;
using DarkDeeds.TaskServiceApp.Services.Implementation;
using DarkDeeds.TaskServiceApp.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.TaskServiceApp.App
{
    public static class StartupExtensions
    {
        public static void AddTaskServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IRepositoryNonDeletable<>), typeof(RepositoryNonDeletable<>));
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ITaskParserService, TaskParserService>();
            services.AddScoped<IRecurrenceCreatorService, RecurrenceCreatorService>();
            services.AddScoped<IDateService, DateService>();
            services.AddScoped<IRecurrenceService, RecurrenceService>();
            services.AddScoped<IPermissionsService, PermissionsService>();
            services.AddScoped<ITaskHubService, TaskHubService>();
        }
        
        public static void AddTaskAutoMapper(this IServiceCollection services)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ModelsMappingProfile>();
            });
        }

        public static void AddTaskDatabase(this IServiceCollection services, IConfiguration configuration)
        {            
            string connectionString = configuration.GetConnectionString("appDb");
            services.AddDbContext<DarkDeedsTaskContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<DbContext, DarkDeedsTaskContext>();
        }
    }
}