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
            services.AddDarkDeedsAppRegistration("web-client-bff");
            services.AddDarkDeedsAuth(Configuration);
            services.AddWebClientBffServices();
            services.AddWebClientBffUseCases();
            services.AddWebClientBffCommunications();
            services.AddWebClientBffData(Configuration);
            services.AddWebClientBffCommonServices();
            services.AddWebClientBffApi();
            services.AddWebClientBffSockets();
            
            services.AddDarkDeedsAmpqSubscriber<TaskUpdatedSubscriber, TaskUpdatedDto>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
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

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<TaskHub>("/ws/task");
            });
        }
    }
}
