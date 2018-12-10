using AutoMapper;
using DarkDeeds.Api.Filters;
using DarkDeeds.AutoMapper;
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
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ITaskService, TaskService>();
            
            // TODO: move to settings
            const string constring = "Server=localhost,1433;Database=darkdeeds;User=sa;Password=Password1";
            services.AddDbContext<DarkDeedsContext>(options => options.UseSqlServer(constring));
            services.AddScoped<DbContext, DarkDeedsContext>();
            
            ConfigIdentity(services);
            
            Mapper.Initialize(cfg => cfg.AddProfile<MappingProfile>());
            
            services.AddMvc(options =>
                {
                    options.Filters.Add(typeof(ExceptionHandlerFilter));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }
        
        public void ConfigIdentity(IServiceCollection services)
        {
            IdentityBuilder builder = services.AddIdentityCore<UserEntity>(options =>
            {
                options.Password.RequiredLength = 8;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<DarkDeedsContext>();

            services.AddScoped<SignInManager<UserEntity>>();
            services.AddScoped<UserManager<UserEntity>>();
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