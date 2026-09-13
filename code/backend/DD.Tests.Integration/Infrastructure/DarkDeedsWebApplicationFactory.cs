using DD.App;
using DD.TelegramClient.Domain.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ServiceTaskDateService = DD.ServiceTask.Domain.Services.IDateService;
using TelegramDateService = DD.TelegramClient.Domain.Services.IDateService;

namespace DD.Tests.Integration.Infrastructure;

internal sealed class DarkDeedsWebApplicationFactory(
    string sharedDbConnectionString,
    IntegrationExternalDependencies externalDependencies)
    : WebApplicationFactory<Startup>
{
    internal const string AuthIssuer = "https://integration-tests.dark-deeds.test";
    private const string AuthAudience = "dark-deeds-integration-tests";
    private const string AuthKey = "dark-deeds-integration-test-signing-key-2026-abcdefghijklmnopqrstuvwxyz";
    private readonly object _clientLock = new();

    public HttpClient CreateTestClient(bool allowAutoRedirect = true)
    {
        lock (_clientLock)
        {
            return CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost"),
                AllowAutoRedirect = allowAutoRedirect,
            });
        }
    }

    public HttpMessageHandler CreateTestServerHandler()
    {
        lock (_clientLock)
        {
            return Server.CreateHandler();
        }
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
                    ["EnableTestHandlers"] = "true",
                    ["Bot"] = "integration-tests",
                });
            })
            .ConfigureServices(services =>
            {
                services.AddDataProtection().UseEphemeralDataProtectionProvider();
                services.RemoveAll<IBotSendMessageService>();
                services.AddSingleton<IBotSendMessageService>(
                    externalDependencies.TelegramMessages);
                services.RemoveAll<ServiceTaskDateService>();
                services.RemoveAll<TelegramDateService>();
                services.AddSingleton<IntegrationTestClock>();
                services.AddSingleton<ServiceTaskDateService>(
                    provider => provider.GetRequiredService<IntegrationTestClock>());
                services.AddSingleton<TelegramDateService>(
                    provider => provider.GetRequiredService<IntegrationTestClock>());
            });
    }
}
