using DarkDeeds.Api.BackgroundServices;
using DarkDeeds.Api.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                .AddIdentity()
                .AddCompression() 
                .ConfigureMvc();

            services.AddDarkDeedsAuth(Configuration);
            services.AddHostedService<HubHeartbeat<TaskHub>>();
            services.AddHealthChecks();
            services.AddSignalR()
                .AddHubOptions<TaskHub>(options => options.EnableDetailedErrors = true);
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseHealthChecks("/healthcheck"); 
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<TaskHub>("/ws/task");
                endpoints.MapFallbackToController("Index", "Home");
                endpoints.MapControllers();
                endpoints.MapControllerRoute("bot", $"api/bot/{Configuration["Bot"]}",
                    new {controller = "Bot", action = "Process"});
            });
        }
    }
}