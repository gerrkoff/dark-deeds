using System.Net.Http.Json;
using DD.WebClientBff.Domain.Dto;

namespace DD.Tests.Integration.Infrastructure.Api;

internal static class SettingsApi
{
    private static readonly Uri SettingsUri = new("api/web/settings", UriKind.Relative);

    internal static Task<HttpResponseMessage> LoadAsync(
        HttpClient client,
        CancellationToken cancellationToken = default)
    {
        return client.GetAsync(SettingsUri, cancellationToken);
    }

    internal static Task<HttpResponseMessage> SaveAsync(
        HttpClient client,
        UserSettingsDto settings,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsJsonAsync(SettingsUri, settings, cancellationToken);
    }
}
