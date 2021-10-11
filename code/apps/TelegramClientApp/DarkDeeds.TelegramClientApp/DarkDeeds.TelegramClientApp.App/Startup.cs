using DarkDeeds.Authentication.DependencyInjection;
using DarkDeeds.Communication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DarkDeeds.TelegramClientApp.App
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
            services.AddDarkDeedsAppRegistration("telegram-client", Configuration);

            services.AddTelegramClientIdentity();
            services.AddTelegramClientCommunications(Configuration);
            services.AddTelegramClientServices(Configuration);
            services.AddTelegramClientDatabase(Configuration);
            services.AddTelegramClientApi();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DarkDeeds.TelegramClientApp v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("bot", $"api/bot/{Configuration["Bot"]}",
                    new {controller = "Bot", action = "Process"});
            });
        }
    }
}