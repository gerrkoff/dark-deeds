using DD.MobileClient.Domain.Dto;

namespace DD.Tests.Integration.Infrastructure.Readers;

internal static class MobileReader
{
    internal static Task<WatchWidgetStatusDto> ReadWidgetAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<WatchWidgetStatusDto>(
            response,
            "mobile widget",
            cancellationToken);
    }

    internal static Task<WatchAppStatusDto> ReadAppAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<WatchAppStatusDto>(
            response,
            "mobile app",
            cancellationToken);
    }
}
