using System.Net;
using DD.Tests.Integration.Infrastructure;
using Xunit;

namespace DD.Tests.Integration;

public sealed class IntegrationEnvironmentTests : IntegrationTestBase
{
    [Fact]
    public async Task SharedEnvironment_StartsApplicationAndProvidesIsolatedIdentifiers()
    {
        var environment = await GetIntegrationEnvironmentAsync();
        using var client = environment.CreateClient();

        using var response = await client.GetAsync(new Uri("/healthcheck", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.StartsWith("dd-integration-tests-", environment.ContainerName, StringComparison.Ordinal);
        Assert.StartsWith("test-", IntegrationEnvironment.CreateUniqueUsername(), StringComparison.Ordinal);
        Assert.NotEqual(
            IntegrationEnvironment.CreateUniqueTaskUid(),
            IntegrationEnvironment.CreateUniqueTaskUid());
    }
}
