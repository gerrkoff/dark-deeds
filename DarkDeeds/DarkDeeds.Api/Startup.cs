using DarkDeeds.Api.BackgroundServices;
using DarkDeeds.Api.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DarkDeeds.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .RegisterServices(Configuration)
                .ConfigureAutoMapper()
                .ConfigureDatabase(Configuration)
                .ConfigureSettings(Configuration)
                .ConfigureAuthentication(Configuration)
                .AddIdentity()
                .AddCompression() 
                .ConfigureMvc();

            services.AddHostedService<HubHeartbeat<TaskHub>>();
            services.AddHealthChecks();
//            services.AddHttpsRedirection(options => options.HttpsPort = 443);
            services.AddSignalR()
                .AddHubOptions<TaskHub>(options => options.EnableDetailedErrors = true);
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(builder => builder
                    .SetIsOriginAllowed(origin => origin.EndsWith(":3000"))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            }
            else
            {
                app.UseHsts();
                app.UseCors(builder => builder
                    .WithOrigins("http://grkf.ru")
                    .WithMethods("GET"));
            }

            app.UseHealthChecks("/healthcheck"); 
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSignalR(options =>
            {
                options.MapHub<TaskHub>("/ws/task");
            });
            app.UseMvc(routes =>
            {
                routes
                    .MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}")
                    .MapRoute(
                        name: "bot",
                        template: $"api/bot/{Configuration["Bot"]}",
                        defaults: new {controller = "Bot", action = "Process"})
                    .MapSpaFallbackRoute(
                        name: "spa-fallback",
                        defaults: new {controller = "Home", action = "Index"});
            });
        }
    }
}