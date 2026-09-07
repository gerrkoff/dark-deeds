using DD.App;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DD.Tests.Integration.Infrastructure;

public sealed class DarkDeedsWebApplicationFactory(string sharedDbConnectionString) : WebApplicationFactory<Startup>
{
    private const string AuthIssuer = "https://integration-tests.dark-deeds.test";
    private const string AuthAudience = "dark-deeds-integration-tests";
    private const string AuthKey = "dark-deeds-integration-test-signing-key-2026-abcdefghijklmnopqrstuvwxyz";

    public HttpClient CreateTestClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost"),
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment("Testing")
            .ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:sharedDb"] = sharedDbConnectionString,
                    ["Auth:Issuer"] = AuthIssuer,
                    ["Auth:Audience"] = AuthAudience,
                    ["Auth:Key"] = AuthKey,
                    ["Auth:Lifetime"] = "2880",
                    ["OAuth:IssuerBaseUrl"] = AuthIssuer,
                    ["OAuth:AccessTokenLifetimeMinutes"] = "60",
                    ["OAuth:RefreshTokenLifetimeDays"] = "30",
                    ["OAuth:ScopesSupported:0"] = "mcp",
                    ["Monitoring:MetricsEnabled"] = "false",
                    ["EnableTelegramIntegration"] = "false",
                    ["EnableTestHandlers"] = "false",
                    ["Bot"] = "integration-tests",
                });
            })
            .ConfigureServices(services =>
            {
                services.AddDataProtection().UseEphemeralDataProtectionProvider();
            });
    }
}
