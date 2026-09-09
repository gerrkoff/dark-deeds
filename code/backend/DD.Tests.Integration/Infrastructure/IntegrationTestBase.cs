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

    protected static Task<T> ExecuteScopedAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        return IntegrationEnvironmentLifetime.ExecuteScopedAsync(action);
    }

    protected static Task<TestUserClient> CreateUserClientAsync()
    {
        return TestUserClient.CreateAsync();
    }
}
