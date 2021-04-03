using DarkDeeds.WebClientBffApp.App.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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
            services.AddWebClientBffServices();
            services.AddWebClientBffAutoMapper();
            services.AddWebClientBffDatabase(Configuration);
            services.AddDarkDeedsAuth(Configuration);

            services.AddHostedService<HubHeartbeat<TaskHub>>();
            services.AddSignalR()
                .AddHubOptions<TaskHub>(options => options.EnableDetailedErrors = true);
            services.AddControllers(options =>
            {
                var authRequired = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(authRequired));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DarkDeeds.WebClientBffApp", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DarkDeeds.WebClientBffApp v1");
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
