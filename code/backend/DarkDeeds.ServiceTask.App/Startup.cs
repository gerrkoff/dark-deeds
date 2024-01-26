using DarkDeeds.AppMetrics;
using DarkDeeds.Authentication;
using DarkDeeds.Common.Web;
using DarkDeeds.Communication;
using DarkDeeds.ServiceTask.Communication;
using DarkDeeds.ServiceTask.ContractImpl;
using DarkDeeds.ServiceTask.ContractImpl.Contract;
using DD.ServiceTask.Details.Data;
using DD.TaskService.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DarkDeeds.ServiceTask.App
{
    public class Startup
    {
        public const string App = "task-service";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDarkDeedsAuth(Configuration);
            services.AddDarkDeedsAppRegistration(App, Configuration, true);
            services.AddDarkDeedsAppMetrics(Configuration);

            services.AddTaskServices();
            services.AddTaskCommunication();
            services.AddTaskDatabase(Configuration);
            services.AddTaskServiceContractImpl();
            services.AddTaskServiceApi();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestLogging();
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
            app.UseDarkDeedsAppMetrics();
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
                endpoints.MapDarkDeedsAppMetrics();
            });
        }
    }
}
