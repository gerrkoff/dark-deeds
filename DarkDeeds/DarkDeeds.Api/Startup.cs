using AutoMapper;
using DarkDeeds.Api.Filters;
using DarkDeeds.AutoMapper;
using DarkDeeds.Common.Settings;
using DarkDeeds.Data.Context;
using DarkDeeds.Data.Entity;
using DarkDeeds.Data.Repository;
using DarkDeeds.Services.Implementation;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                .RegisterServices()
                .ConfigureAutomapper()
                .ConfigureDatabase()
                .ConfigureSettings(Configuration)
                .ConfigureAuthentication(Configuration)
                .AddIdentity()
//                .AddCompression()    TODO: enable it 
                .ConfigureMvc();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            }
            else
            {
                app.UseHsts();
            }
            
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
 
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}