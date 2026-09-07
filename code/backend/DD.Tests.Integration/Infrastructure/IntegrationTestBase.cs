using System.Globalization;
using System.Net.Http.Json;
using DD.Shared.Details.Abstractions.Dto;

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

    protected static async Task<TaskDto[]> ReadTasksAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TaskDto[]>()
            ?? throw new InvalidOperationException("The task response was empty.");
    }

    protected static Uri CreateTasksUri(DateTime from)
    {
        var value = from.ToString("O", CultureInfo.InvariantCulture);
        return new Uri(
            $"api/task/tasks?from={Uri.EscapeDataString(value)}",
            UriKind.Relative);
    }
}
