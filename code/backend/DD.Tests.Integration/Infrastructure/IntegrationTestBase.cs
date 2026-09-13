using DD.Tests.Integration.Infrastructure.Clients;
using DD.Tests.Integration.Infrastructure.ExternalDependencies;

namespace DD.Tests.Integration.Infrastructure;

public abstract class IntegrationTestBase
{
    protected static Task<HttpClient> CreateClientAsync()
    {
        return IntegrationEnvironmentLifetime.CreateClientAsync();
    }

    protected static Task<TestUserClient> CreateUserClientAsync()
    {
        return TestUserClient.CreateAsync();
    }

    protected static Task<TestBotSendMessageService> GetBotMessagesAsync()
    {
        return IntegrationEnvironmentLifetime.GetBotMessagesAsync();
    }
}
