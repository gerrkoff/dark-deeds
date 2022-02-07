using DarkDeeds.AppMetrics;
using DarkDeeds.Authentication;
using DarkDeeds.Backend.Data;
using DarkDeeds.Common.Validation;
using DarkDeeds.Communication;
using DarkDeeds.ServiceAuth.ContractImpl;
using DarkDeeds.ServiceAuth.ContractImpl.Contract;
using DarkDeeds.ServiceAuth.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DarkDeeds.ServiceAuth.App
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
            services.AddDarkDeedsValidation();
            services.AddDarkDeedsAppRegistration("auth-service", Configuration, true);
            services.AddDarkDeedsAppMetrics(Configuration);
            services.AddBackendDatabase(Configuration);
            services.AddAuthServiceServices();
            services.AddAuthServiceContractImpl();
            services.AddAuthServiceApi();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DarkDeeds.AuthService v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseRouting();
            app.UseDarkDeedsAppMetrics();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<AuthServiceImpl>();
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