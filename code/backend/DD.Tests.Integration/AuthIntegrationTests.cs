using System.Net;
using System.Net.Http.Headers;
using DD.ServiceAuth.Domain.Dto;
using DD.ServiceAuth.Domain.Enums;
using DD.Tests.Integration.Infrastructure;
using DD.Tests.Integration.Infrastructure.Api;
using DD.Tests.Integration.Infrastructure.Readers;
using Xunit;
using static DD.Tests.Integration.Helpers.AuthTestData;

namespace DD.Tests.Integration;

public sealed class AuthIntegrationTests : IntegrationTestBase
{
    private const string Password = "QWERTY123456qwerty!@#$%^";
    private const string WrongPassword = "QWERTY123456qwerty!@#$%wrong";

    [Fact]
    public async Task CurrentUser_Anonymous_ReturnsUnauthenticatedUser()
    {
        using var client = await CreateClientAsync();

        using var response = await AuthApi.GetCurrentUserAsync(client);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var currentUser = await AuthReader.ReadCurrentUserAsync(response);
        Assert.False(currentUser.UserAuthenticated);
        Assert.Null(currentUser.Username);
        Assert.Null(currentUser.Expires);
    }

    [Fact]
    public async Task AccountEndpoints_SignUpSignInCurrentUserAndRenew_Succeed()
    {
        using var client = await CreateClientAsync();
        var username = CreateUniqueUsername("auth");

        using var signUpResponse = await AuthApi.SignUpAsync(
            client,
            new SignUpInfoDto { Username = username, Password = Password });

        Assert.Equal(HttpStatusCode.OK, signUpResponse.StatusCode);
        var signUpResult = await AuthReader.ReadSignUpAsync(signUpResponse);
        Assert.Equal(SignUpResult.Success, signUpResult.Result);
        Assert.False(string.IsNullOrWhiteSpace(signUpResult.Token));

        using var signInResponse = await AuthApi.SignInAsync(
            client,
            new SignInInfoDto { Username = username, Password = Password });

        Assert.Equal(HttpStatusCode.OK, signInResponse.StatusCode);
        var signInResult = await AuthReader.ReadSignInAsync(signInResponse);
        Assert.Equal(SignInResult.Success, signInResult.Result);
        Assert.False(string.IsNullOrWhiteSpace(signInResult.Token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", signInResult.Token);

        using var currentUserResponse = await AuthApi.GetCurrentUserAsync(client);
        Assert.Equal(HttpStatusCode.OK, currentUserResponse.StatusCode);
        var currentUser = await AuthReader.ReadCurrentUserAsync(currentUserResponse);
        Assert.True(currentUser.UserAuthenticated);
        Assert.Equal(username, currentUser.Username);
        Assert.NotNull(currentUser.Expires);

        using var renewResponse = await AuthApi.RenewAsync(client);
        Assert.Equal(HttpStatusCode.OK, renewResponse.StatusCode);
        var renewedToken = await AuthReader.ReadTokenAsync(renewResponse);
        Assert.False(string.IsNullOrWhiteSpace(renewedToken));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", renewedToken);
        using var renewedCurrentUserResponse = await AuthApi.GetCurrentUserAsync(client);
        Assert.Equal(HttpStatusCode.OK, renewedCurrentUserResponse.StatusCode);
        var renewedCurrentUser = await AuthReader.ReadCurrentUserAsync(
            renewedCurrentUserResponse);
        Assert.True(renewedCurrentUser.UserAuthenticated);
        Assert.Equal(username, renewedCurrentUser.Username);
    }

    [Fact]
    public async Task SignIn_ExistingUserWithWrongPassword_ReturnsWrongUsernamePassword()
    {
        using var client = await CreateClientAsync();
        var username = CreateUniqueUsername("wrong-password");

        using var signUpResponse = await AuthApi.SignUpAsync(
            client,
            new SignUpInfoDto { Username = username, Password = Password });
        Assert.Equal(HttpStatusCode.OK, signUpResponse.StatusCode);
        var signUpResult = await AuthReader.ReadSignUpAsync(signUpResponse);
        Assert.Equal(SignUpResult.Success, signUpResult.Result);

        using var response = await AuthApi.SignInAsync(
            client,
            new SignInInfoDto
            {
                Username = username,
                Password = WrongPassword,
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await AuthReader.ReadSignInAsync(response);
        Assert.Equal(SignInResult.WrongUsernamePassword, result.Result);
        Assert.Empty(result.Token);
    }

    [Fact]
    public async Task Renew_Anonymous_ReturnsUnauthorized()
    {
        using var client = await CreateClientAsync();

        using var response = await AuthApi.RenewAsync(client);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SignUp_DuplicateUsername_ReturnsUsernameAlreadyExists()
    {
        using var client = await CreateClientAsync();
        var username = CreateUniqueUsername("duplicate");
        var signUpInfo = new SignUpInfoDto { Username = username, Password = Password };

        using var firstResponse = await AuthApi.SignUpAsync(client, signUpInfo);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        using var duplicateResponse = await AuthApi.SignUpAsync(client, signUpInfo);

        Assert.Equal(HttpStatusCode.OK, duplicateResponse.StatusCode);
        var result = await AuthReader.ReadSignUpAsync(duplicateResponse);
        Assert.Equal(SignUpResult.UsernameAlreadyExists, result.Result);
        Assert.Empty(result.Token);
    }

    [Fact]
    public async Task Tasks_AnonymousRequest_ReturnsUnauthorized()
    {
        using var client = await CreateClientAsync();

        using var response = await TasksApi.LoadAsync(
            client,
            new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
