using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DD.ServiceAuth.Domain.Dto;
using DD.ServiceAuth.Domain.Enums;

namespace DD.Tests.Integration.Infrastructure;

public sealed class TestUserClient : IAsyncDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private TestUserClient(HttpClient httpClient, string username, string password, string token)
    {
        HttpClient = httpClient;
        Username = username;
        Password = password;
        Token = token;
    }

    public string Username { get; }

    public string Password { get; }

    public string Token { get; }

    public HttpClient HttpClient { get; }

    public static async Task<TestUserClient> CreateAsync(
        IntegrationEnvironment environment,
        CancellationToken cancellationToken = default)
    {
        var httpClient = environment.CreateClient();
        var shouldDisposeClient = true;

        try
        {
            var username = IntegrationEnvironment.CreateUniqueUsername();
            const string password = "QWERTY123456qwerty!@#$%^";
            using var response = await httpClient.PostAsJsonAsync(
                "api/auth/account/signup",
                new SignUpInfoDto
                {
                    Username = username,
                    Password = password,
                },
                JsonOptions,
                cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<SignUpResultDto>(
                JsonOptions,
                cancellationToken);
            if (result is null || result.Result != SignUpResult.Success || string.IsNullOrEmpty(result.Token))
            {
                throw new InvalidOperationException("The integration test user could not be created.");
            }

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", result.Token);
            shouldDisposeClient = false;
            return new TestUserClient(httpClient, username, password, result.Token);
        }
        finally
        {
            if (shouldDisposeClient)
                httpClient.Dispose();
        }
    }

    public ValueTask DisposeAsync()
    {
        HttpClient.Dispose();
        return ValueTask.CompletedTask;
    }
}
