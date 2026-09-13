using System.Net;
using DD.Tests.Integration.Infrastructure;
using DD.Tests.Integration.Infrastructure.Api;
using DD.Tests.Integration.Infrastructure.Readers;
using Xunit;

namespace DD.Tests.Integration;

public sealed class StartupIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task FreshEnvironment_StartsApplicationAndServesAnonymousHealthAndBuildInfo()
    {
        using var client = await CreateClientAsync();

        using var healthResponse = await SystemApi.GetHealthAsync(client);

        Assert.Equal(HttpStatusCode.OK, healthResponse.StatusCode);
        Assert.Equal("Healthy", await SystemReader.ReadHealthAsync(healthResponse));

        using var buildInfoResponse = await SystemApi.GetBuildInfoAsync(client);
        Assert.Equal(HttpStatusCode.OK, buildInfoResponse.StatusCode);

        using var buildInfo = await SystemReader.ReadBuildInfoAsync(buildInfoResponse);
        Assert.True(buildInfo.RootElement.TryGetProperty("appVersion", out var appVersion));
        Assert.False(string.IsNullOrWhiteSpace(appVersion.GetString()));
    }
}
