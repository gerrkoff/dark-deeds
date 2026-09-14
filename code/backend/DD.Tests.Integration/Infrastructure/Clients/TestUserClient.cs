using System.Net.Http.Headers;
using DD.ServiceAuth.Domain.Dto;
using DD.ServiceAuth.Domain.Enums;
using DD.Tests.Integration.Helpers;
using DD.Tests.Integration.Infrastructure.Api;
using DD.Tests.Integration.Infrastructure.Readers;

namespace DD.Tests.Integration.Infrastructure.Clients;

public sealed class TestUserClient : IAsyncDisposable
{
    private TestUserClient(HttpClient httpClient, string token)
    {
        HttpClient = httpClient;
        Token = token;
    }

    public string Token { get; }

    public HttpClient HttpClient { get; }

    public static async Task<TestUserClient> CreateAsync(
        CancellationToken cancellationToken = default)
    {
        var httpClient = await IntegrationEnvironmentLifetime.CreateClientAsync();
        var shouldDisposeClient = true;

        try
        {
            var username = AuthHelper.CreateUniqueUsername();
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
            return new TestUserClient(httpClient, result.Token);
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
