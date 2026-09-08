using System.Net;
using System.Net.Http.Json;
using DD.Tests.Integration.Infrastructure;
using DD.WebClientBff.Domain.Dto;
using Xunit;

namespace DD.Tests.Integration;

public sealed class SettingsIntegrationTests : IntegrationTestBase
{
    private static readonly Uri SettingsUri = new("api/web/settings", UriKind.Relative);

    [Fact]
    public async Task GetSettings_NewUser_ReturnsShowCompletedFalse()
    {
        await using var user = await CreateUserClientAsync();

        using var response = await user.HttpClient.GetAsync(SettingsUri);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var settings = await response.Content.ReadFromJsonAsync<UserSettingsDto>();
        Assert.NotNull(settings);
        Assert.False(settings.ShowCompleted);
    }

    [Fact]
    public async Task PostSettings_ValuesPersistThroughLaterGet()
    {
        await using var user = await CreateUserClientAsync();

        using var saveTrueResponse = await user.HttpClient.PostAsJsonAsync(
            "api/web/settings",
            new UserSettingsDto { ShowCompleted = true });
        Assert.Equal(HttpStatusCode.OK, saveTrueResponse.StatusCode);

        using var getTrueResponse = await user.HttpClient.GetAsync(SettingsUri);
        Assert.Equal(HttpStatusCode.OK, getTrueResponse.StatusCode);
        var trueSettings = await getTrueResponse.Content.ReadFromJsonAsync<UserSettingsDto>();
        Assert.NotNull(trueSettings);
        Assert.True(trueSettings.ShowCompleted);

        using var saveFalseResponse = await user.HttpClient.PostAsJsonAsync(
            "api/web/settings",
            new UserSettingsDto { ShowCompleted = false });
        Assert.Equal(HttpStatusCode.OK, saveFalseResponse.StatusCode);

        using var getFalseResponse = await user.HttpClient.GetAsync(SettingsUri);
        Assert.Equal(HttpStatusCode.OK, getFalseResponse.StatusCode);
        var falseSettings = await getFalseResponse.Content.ReadFromJsonAsync<UserSettingsDto>();
        Assert.NotNull(falseSettings);
        Assert.False(falseSettings.ShowCompleted);
    }

    [Fact]
    public async Task Settings_TwoUsers_AreIsolated()
    {
        await using var firstUser = await CreateUserClientAsync();
        await using var secondUser = await CreateUserClientAsync();

        using var saveResponse = await firstUser.HttpClient.PostAsJsonAsync(
            "api/web/settings",
            new UserSettingsDto { ShowCompleted = true });
        Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);

        using var firstResponse = await firstUser.HttpClient.GetAsync(SettingsUri);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        var firstSettings = await firstResponse.Content.ReadFromJsonAsync<UserSettingsDto>();
        Assert.NotNull(firstSettings);
        Assert.True(firstSettings.ShowCompleted);

        using var secondResponse = await secondUser.HttpClient.GetAsync(SettingsUri);
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        var secondSettings = await secondResponse.Content.ReadFromJsonAsync<UserSettingsDto>();
        Assert.NotNull(secondSettings);
        Assert.False(secondSettings.ShowCompleted);
    }
}
