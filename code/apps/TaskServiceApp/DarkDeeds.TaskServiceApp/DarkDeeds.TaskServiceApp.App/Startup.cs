using System.Collections.Generic;
using DarkDeeds.Authentication.DependencyInjection;
using DarkDeeds.Communication;
using DarkDeeds.TaskServiceApp.Communication;
using DarkDeeds.TaskServiceApp.Communication.Publishers;
using DarkDeeds.TaskServiceApp.ContractImpl;
using DarkDeeds.TaskServiceApp.ContractImpl.Contract;
using DarkDeeds.TaskServiceApp.Models.Dto;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DarkDeeds.TaskServiceApp.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDarkDeedsAuth(Configuration);
            services.AddDarkDeedsAppRegistration("task-service");
            services.AddDarkDeedsAmpqPublisher<ITaskUpdatedPublisher, TaskUpdatedPublisher, ICollection<TaskDto>>();
            
            services.AddTaskServices(Configuration);
            services.AddTaskAutoMapper();
            services.AddTaskDatabase(Configuration);
            services.AddTaskServiceContractImpl();
            services.AddTaskServiceApi();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DarkDeeds.TaskService v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ParserServiceImpl>();
                endpoints.MapGrpcService<TaskServiceImpl>();
                endpoints.MapGrpcService<RecurrenceServiceImpl>();
                if (!env.IsProduction())
                {
                    endpoints.MapGrpcReflectionService();
                }
                endpoints.MapControllers();
            });
        }
    }
}