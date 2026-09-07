namespace DD.Tests.Integration.Infrastructure;

public abstract class IntegrationTestBase
{
    protected static Task<IntegrationEnvironment> GetIntegrationEnvironmentAsync()
    {
        return IntegrationEnvironment.GetAsync();
    }

    protected static async Task<HttpClient> CreateClientAsync()
    {
        var environment = await GetIntegrationEnvironmentAsync();
        return environment.CreateClient();
    }

    protected static async Task<TestUserClient> CreateUserClientAsync()
    {
        var environment = await GetIntegrationEnvironmentAsync();
        return await TestUserClient.CreateAsync(environment);
    }
}
