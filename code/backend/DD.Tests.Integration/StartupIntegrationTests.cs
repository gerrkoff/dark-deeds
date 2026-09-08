using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DD.Tests.Integration.Infrastructure;
using Xunit;

namespace DD.Tests.Integration;

public sealed class StartupIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task FreshEnvironment_StartsApplicationAndServesAnonymousHealthAndBuildInfo()
    {
        using var client = await CreateClientAsync();

        using var healthResponse = await client.GetAsync(new Uri("healthcheck", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, healthResponse.StatusCode);
        Assert.Equal("Healthy", (await healthResponse.Content.ReadAsStringAsync()).Trim());

        using var buildInfoResponse =
            await client.GetAsync(new Uri("api/be/build-info", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, buildInfoResponse.StatusCode);

        using var buildInfo = await buildInfoResponse.Content.ReadFromJsonAsync<JsonDocument>();

        Assert.NotNull(buildInfo);
        Assert.True(buildInfo.RootElement.TryGetProperty("appVersion", out var appVersion));
        Assert.False(string.IsNullOrWhiteSpace(appVersion.GetString()));
    }
}
