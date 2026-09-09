using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using DD.MobileClient.Domain.Dto;

namespace DD.Tests.Integration.Helpers;

internal static class MobileHelper
{
    internal const string WatchRoute = "api/mobile/Watch";

    internal static string CreateUniqueMobileKey()
    {
        return $"mobile-{Guid.NewGuid():N}";
    }

    internal static string GetUserId(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return jwt.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
    }

    internal static Uri CreateWidgetUri(string mobileKey)
    {
        return CreateWatchUri(mobileKey, "widget");
    }

    internal static Uri CreateAppUri(string mobileKey)
    {
        return CreateWatchUri(mobileKey, "app");
    }

    internal static async Task<WatchWidgetStatusDto> ReadWidgetAsync(
        HttpClient client,
        string mobileKey)
    {
        return await ReadAsync<WatchWidgetStatusDto>(client, CreateWidgetUri(mobileKey));
    }

    internal static async Task<WatchAppStatusDto> ReadAppAsync(
        HttpClient client,
        string mobileKey)
    {
        return await ReadAsync<WatchAppStatusDto>(client, CreateAppUri(mobileKey));
    }

    internal static async Task<T> PollUntilAsync<T>(
        Func<Task<T>> loadAsync,
        Func<T, bool> predicate,
        TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(loadAsync);
        ArgumentNullException.ThrowIfNull(predicate);

        var started = Stopwatch.GetTimestamp();
        while (true)
        {
            var remaining = timeout - Stopwatch.GetElapsedTime(started);
            if (remaining <= TimeSpan.Zero)
                break;

            var result = await loadAsync().WaitAsync(remaining);
            if (predicate(result))
                return result;

            remaining = timeout - Stopwatch.GetElapsedTime(started);
            if (remaining <= TimeSpan.Zero)
                break;

            var pollingDelay = TimeSpan.FromMilliseconds(50);
            await Task.Delay(pollingDelay < remaining ? pollingDelay : remaining);
        }

        throw new TimeoutException($"The expected mobile payload was not observed within {timeout}.");
    }

    private static Uri CreateWatchUri(string mobileKey, string endpoint)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mobileKey);
        return new Uri(
            $"{WatchRoute}/{Uri.EscapeDataString(mobileKey)}/{endpoint}",
            UriKind.Relative);
    }

    private static async Task<T> ReadAsync<T>(HttpClient client, Uri uri)
    {
        using var response = await client.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>()
               ?? throw new InvalidOperationException("The mobile response was empty.");
    }
}
