using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DD.ServiceAuth.Details.Web;
using DD.ServiceAuth.Domain.Dto;
using DD.ServiceAuth.Domain.Enums;
using DD.Tests.Integration.Infrastructure;
using Xunit;
using static DD.Tests.Integration.Helpers.Helper;

namespace DD.Tests.Integration;

public sealed class AuthIntegrationTests : IntegrationTestBase
{
    private const string Password = "QWERTY123456qwerty!@#$%^";
    private const string WrongPassword = "QWERTY123456qwerty!@#$%wrong";

    [Fact]
    public async Task CurrentUser_Anonymous_ReturnsUnauthenticatedUser()
    {
        using var client = await CreateClientAsync();

        using var response = await client.GetAsync(new Uri("api/auth/account", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var currentUser = await response.Content.ReadFromJsonAsync<CurrentUserDto>();
        Assert.NotNull(currentUser);
        Assert.False(currentUser.UserAuthenticated);
        Assert.Null(currentUser.Username);
        Assert.Null(currentUser.Expires);
    }

    [Fact]
    public async Task AccountEndpoints_SignUpSignInCurrentUserAndRenew_Succeed()
    {
        using var client = await CreateClientAsync();
        var username = CreateUniqueUsername("auth");

        using var signUpResponse = await client.PostAsJsonAsync(
            "api/auth/account/signup",
            new SignUpInfoDto { Username = username, Password = Password });

        Assert.Equal(HttpStatusCode.OK, signUpResponse.StatusCode);
        var signUpResult = await signUpResponse.Content.ReadFromJsonAsync<SignUpResultDto>();
        Assert.NotNull(signUpResult);
        Assert.Equal(SignUpResult.Success, signUpResult.Result);
        Assert.False(string.IsNullOrWhiteSpace(signUpResult.Token));

        using var signInResponse = await client.PostAsJsonAsync(
            "api/auth/account/signin",
            new SignInInfoDto { Username = username, Password = Password });

        Assert.Equal(HttpStatusCode.OK, signInResponse.StatusCode);
        var signInResult = await signInResponse.Content.ReadFromJsonAsync<SignInResultDto>();
        Assert.NotNull(signInResult);
        Assert.Equal(SignInResult.Success, signInResult.Result);
        Assert.False(string.IsNullOrWhiteSpace(signInResult.Token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", signInResult.Token);

        using var currentUserResponse =
            await client.GetAsync(new Uri("api/auth/account", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, currentUserResponse.StatusCode);
        var currentUser = await currentUserResponse.Content.ReadFromJsonAsync<CurrentUserDto>();
        Assert.NotNull(currentUser);
        Assert.True(currentUser.UserAuthenticated);
        Assert.Equal(username, currentUser.Username);
        Assert.NotNull(currentUser.Expires);

        using var renewResponse =
            await client.PostAsync(new Uri("api/auth/account/renew", UriKind.Relative), content: null);
        Assert.Equal(HttpStatusCode.OK, renewResponse.StatusCode);
        var renewedToken = (await renewResponse.Content.ReadAsStringAsync()).Trim();
        Assert.False(string.IsNullOrWhiteSpace(renewedToken));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", renewedToken);
        using var renewedCurrentUserResponse =
            await client.GetAsync(new Uri("api/auth/account", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, renewedCurrentUserResponse.StatusCode);
        var renewedCurrentUser =
            await renewedCurrentUserResponse.Content.ReadFromJsonAsync<CurrentUserDto>();
        Assert.NotNull(renewedCurrentUser);
        Assert.True(renewedCurrentUser.UserAuthenticated);
        Assert.Equal(username, renewedCurrentUser.Username);
    }

    [Fact]
    public async Task SignIn_ExistingUserWithWrongPassword_ReturnsWrongUsernamePassword()
    {
        using var client = await CreateClientAsync();
        var username = CreateUniqueUsername("wrong-password");

        using var signUpResponse = await client.PostAsJsonAsync(
            "api/auth/account/signup",
            new SignUpInfoDto { Username = username, Password = Password });
        Assert.Equal(HttpStatusCode.OK, signUpResponse.StatusCode);
        var signUpResult = await signUpResponse.Content.ReadFromJsonAsync<SignUpResultDto>();
        Assert.NotNull(signUpResult);
        Assert.Equal(SignUpResult.Success, signUpResult.Result);

        using var response = await client.PostAsJsonAsync(
            "api/auth/account/signin",
            new SignInInfoDto
            {
                Username = username,
                Password = WrongPassword,
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SignInResultDto>();
        Assert.NotNull(result);
        Assert.Equal(SignInResult.WrongUsernamePassword, result.Result);
        Assert.Empty(result.Token);
    }

    [Fact]
    public async Task Renew_Anonymous_ReturnsUnauthorized()
    {
        using var client = await CreateClientAsync();

        using var response =
            await client.PostAsync(new Uri("api/auth/account/renew", UriKind.Relative), content: null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SignUp_DuplicateUsername_ReturnsUsernameAlreadyExists()
    {
        using var client = await CreateClientAsync();
        var username = CreateUniqueUsername("duplicate");
        var signUpInfo = new SignUpInfoDto { Username = username, Password = Password };

        using var firstResponse = await client.PostAsJsonAsync("api/auth/account/signup", signUpInfo);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        using var duplicateResponse = await client.PostAsJsonAsync(
            "api/auth/account/signup",
            signUpInfo);

        Assert.Equal(HttpStatusCode.OK, duplicateResponse.StatusCode);
        var result = await duplicateResponse.Content.ReadFromJsonAsync<SignUpResultDto>();
        Assert.NotNull(result);
        Assert.Equal(SignUpResult.UsernameAlreadyExists, result.Result);
        Assert.Empty(result.Token);
    }

    [Fact]
    public async Task Tasks_AnonymousRequest_ReturnsUnauthorized()
    {
        using var client = await CreateClientAsync();

        using var response = await client.GetAsync(
            new Uri(
                "api/task/tasks?from=2026-01-01T00%3A00%3A00.0000000Z",
                UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
