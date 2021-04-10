using DarkDeeds.Authentication.DependencyInjection;
using DarkDeeds.AuthServiceApp.ContractImpl;
using DarkDeeds.AuthServiceApp.ContractImpl.Contract;
using DarkDeeds.AuthServiceApp.Data;
using DarkDeeds.AuthServiceApp.Services;
using DarkDeeds.Common.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DarkDeeds.AuthServiceApp.App
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
            
            services.AddAuthServiceServices();
            services.AddAuthServiceContractImpl();
            services.AddAuthServiceIdentity();
            services.AddAuthServiceDatabase(Configuration);
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
            });
        }
    }
}