using DarkDeeds.AppMetrics;
using DarkDeeds.Authentication.DependencyInjection;
using DarkDeeds.CommonWeb;
using DarkDeeds.Communication;
using DarkDeeds.TelegramClientApp.Web;
using DarkDeeds.WebClientBffApp.Web;
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
            services.AddTelegramClient(Configuration);
            services.AddWebClient(Configuration);
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