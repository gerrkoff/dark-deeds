using System.IO.Compression;
using DD.App.Dto;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;

namespace DD.App;

public static class StartupExtensions
{
    public static void AddCompression(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
            options.Providers.Add<BrotliCompressionProvider>();
            options.MimeTypes = ["application/json"]; // other assets pre-compressed
        });
        services.Configure<GzipCompressionProviderOptions>(o =>
        {
            o.Level = CompressionLevel.Fastest; // or Optimal if acceptable CPU tradeoff
        });
        services.Configure<BrotliCompressionProviderOptions>(o => { o.Level = CompressionLevel.Fastest; });
    }

    public static void AddSwagger(this IServiceCollection services)
    {
        var buildInfo = new BuildInfoDto(typeof(Startup));
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DarkDeeds.Backend",
                Version = buildInfo.AppVersion,
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    []
                },
            });
        });
    }
}
