using DarkDeeds.AppMetrics;
using DarkDeeds.Authentication.DependencyInjection;
using DarkDeeds.Communication;
using DarkDeeds.WebClientBffApp.App.BackgroundServices;
using DarkDeeds.WebClientBffApp.App.Hubs;
using DarkDeeds.WebClientBffApp.Communication;
using DarkDeeds.WebClientBffApp.Data;
using DarkDeeds.WebClientBffApp.Services;
using DarkDeeds.WebClientBffApp.Services.Dto;
using DarkDeeds.WebClientBffApp.UseCases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DarkDeeds.CommonWeb;

namespace DarkDeeds.WebClientBffApp.App
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
            services.AddDarkDeedsAppRegistration("web-client-bff", Configuration);
            services.AddDarkDeedsAuth(Configuration);
            services.AddDarkDeedsAppMetrics(Configuration);
            services.AddWebClientBffServices();
            services.AddWebClientBffUseCases();
            services.AddWebClientBffCommunications(Configuration);
            services.AddWebClientBffData(Configuration);
            services.AddWebClientBffCommonServices();
            services.AddWebClientBffApi();
            services.AddWebClientBffSockets();
            
            services.AddDarkDeedsAmpqSubscriber<TaskUpdatedSubscriber, TaskUpdatedDto>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseDarkDeedsAppMetrics();
            app.UseDarkDeedsExceptionHandler(env.IsProduction());
            
            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DarkDeeds.WebClientBff v1");
                    c.RoutePrefix = string.Empty;
                });
                app.UseCors(builder => builder
                    .SetIsOriginAllowed(origin => origin.EndsWith(":3000"))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<TaskHub>("/ws/task");
                endpoints.MapDarkDeedsAppMetrics();
            });
        }
    }
}
