using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DarkDeeds.E2eTests.Models;

namespace DarkDeeds.E2eTests.Backend;

public static class BackendApi
{
    private static readonly Uri BackendUrl =
        new(Environment.GetEnvironmentVariable("BE_URL") ?? "http://localhost:5000");

    public static HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
        };

        return new HttpClient(handler)
        {
            BaseAddress = BackendUrl,
        };
    }

    public static async Task<TestUserDto> CreateUserAsync()
    {
        using var client = CreateHttpClient();
        var result = await client.PostAsync(new Uri("api/test/CreateTestUser", UriKind.Relative), null);
        result.EnsureSuccessStatusCode();
        var content = await result.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<TestUserDto>(content, JsonOptions.I);

        ArgumentNullException.ThrowIfNull(user);
        return user;
    }

    public static async Task<string> SignInAsync(TestUserDto user)
    {
        using var client = CreateHttpClient();
        var payload = JsonSerializer.Serialize(new { username = user.Username, password = user.Password });
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(new Uri("api/auth/account/signin", UriKind.Relative), content);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SignInResultDto>(json, JsonOptions.I);

        ArgumentNullException.ThrowIfNull(result);
        return result.Token;
    }

    public static async Task<IReadOnlyCollection<string>> GetTaskTitlesAsync(TestUserDto user)
    {
        var token = await SignInAsync(user);

        using var client = CreateHttpClient();
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            new Uri("api/task/tasks?from=2000-01-01T00:00:00Z", UriKind.Relative));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var tasks = JsonSerializer.Deserialize<List<BackendTaskDto>>(content, JsonOptions.I) ?? [];

        return tasks.Where(task => !task.Deleted).Select(task => task.Title).ToList();
    }
}
