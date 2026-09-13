using System.Net;
using DD.Tests.Integration.Infrastructure;
using DD.Tests.Integration.Infrastructure.Api;
using DD.Tests.Integration.Infrastructure.Readers;
using DD.WebClientBff.Domain.Dto;
using Xunit;

namespace DD.Tests.Integration;

public sealed class SettingsIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task GetSettings_NewUser_ReturnsShowCompletedFalse()
    {
        await using var user = await CreateUserClientAsync();

        using var response = await SettingsApi.LoadAsync(user.HttpClient);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var settings = await SettingsReader.ReadAsync(response);
        Assert.False(settings.ShowCompleted);
    }

    [Fact]
    public async Task PostSettings_ValuesPersistThroughLaterGet()
    {
        await using var user = await CreateUserClientAsync();

        using var saveTrueResponse = await SettingsApi.SaveAsync(
            user.HttpClient,
            new UserSettingsDto { ShowCompleted = true });
        Assert.Equal(HttpStatusCode.OK, saveTrueResponse.StatusCode);

        using var getTrueResponse = await SettingsApi.LoadAsync(user.HttpClient);
        Assert.Equal(HttpStatusCode.OK, getTrueResponse.StatusCode);
        var trueSettings = await SettingsReader.ReadAsync(getTrueResponse);
        Assert.True(trueSettings.ShowCompleted);

        using var saveFalseResponse = await SettingsApi.SaveAsync(
            user.HttpClient,
            new UserSettingsDto { ShowCompleted = false });
        Assert.Equal(HttpStatusCode.OK, saveFalseResponse.StatusCode);

        using var getFalseResponse = await SettingsApi.LoadAsync(user.HttpClient);
        Assert.Equal(HttpStatusCode.OK, getFalseResponse.StatusCode);
        var falseSettings = await SettingsReader.ReadAsync(getFalseResponse);
        Assert.False(falseSettings.ShowCompleted);
    }

    [Fact]
    public async Task Settings_TwoUsers_AreIsolated()
    {
        await using var firstUser = await CreateUserClientAsync();
        await using var secondUser = await CreateUserClientAsync();

        using var saveResponse = await SettingsApi.SaveAsync(
            firstUser.HttpClient,
            new UserSettingsDto { ShowCompleted = true });
        Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);

        using var firstResponse = await SettingsApi.LoadAsync(firstUser.HttpClient);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        var firstSettings = await SettingsReader.ReadAsync(firstResponse);
        Assert.True(firstSettings.ShowCompleted);

        using var secondResponse = await SettingsApi.LoadAsync(secondUser.HttpClient);
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        var secondSettings = await SettingsReader.ReadAsync(secondResponse);
        Assert.False(secondSettings.ShowCompleted);
    }
}
