using System.Net.Http.Headers;
using DD.ServiceAuth.Domain.Dto;
using DD.ServiceAuth.Domain.Enums;
using DD.Tests.Integration.Infrastructure.Api;
using DD.Tests.Integration.Infrastructure.Readers;
using static DD.Tests.Integration.Helpers.AuthTestData;

namespace DD.Tests.Integration.Infrastructure.Clients;

public sealed class TestUserClient : IAsyncDisposable
{
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
        CancellationToken cancellationToken = default)
    {
        var httpClient = await IntegrationEnvironmentLifetime.CreateClientAsync();
        var shouldDisposeClient = true;

        try
        {
            var username = CreateUniqueUsername();
            const string password = "QWERTY123456qwerty!@#$%^";
            using var response = await AuthApi.SignUpAsync(
                httpClient,
                new SignUpInfoDto
                {
                    Username = username,
                    Password = password,
                },
                cancellationToken);

            var result = await AuthReader.ReadSignUpAsync(response, cancellationToken);
            if (result.Result != SignUpResult.Success || string.IsNullOrEmpty(result.Token))
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
