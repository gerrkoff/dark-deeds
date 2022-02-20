using DarkDeeds.AppMetrics;
using DarkDeeds.Authentication;
using DarkDeeds.Backend.Data;
using DarkDeeds.Common.Web;
using DarkDeeds.Communication;
using DarkDeeds.ServiceAuth.Consumers.Impl;
using DarkDeeds.ServiceTask.Consumers.Impl;
using DarkDeeds.TelegramClient.Web;
using DarkDeeds.WebClientBff.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DarkDeeds.Backend.App
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
            services.AddDarkDeedsAppRegistration("backend", Configuration);
            services.AddDarkDeedsAppMetrics(Configuration);
            services.AddDarkDeedsTestControllers();

            services.AddBackendApi();
            services.AddBackendDatabase(Configuration);
            services.AddTelegramClient(Configuration);
            services.AddWebClient(Configuration);
            services.AddTaskServiceApp(Configuration);
            services.AddAuthServiceApp(Configuration);
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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DarkDeeds.Backend v1");
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
                endpoints.MapDarkDeedsAppMetrics();
                endpoints.MapTelegramClientCustomRoutes(Configuration);
                endpoints.MapWebClientCustomRoutes();
            });
        }
    }
}
