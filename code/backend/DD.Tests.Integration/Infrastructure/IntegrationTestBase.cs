namespace DD.Tests.Integration.Infrastructure;

public abstract class IntegrationTestBase
{
    protected static Task<HttpClient> CreateClientAsync()
    {
        return IntegrationEnvironmentLifetime.CreateClientAsync();
    }

    protected static Task<HttpMessageHandler> CreateSignalRHandlerAsync()
    {
        return IntegrationEnvironmentLifetime.CreateSignalRHandlerAsync();
    }

    protected static Task<TestUserClient> CreateUserClientAsync()
    {
        return TestUserClient.CreateAsync();
    }
}
