using DD.WebClientBff.Domain.Dto;

namespace DD.Tests.Integration.Infrastructure.Readers;

internal static class SettingsReader
{
    internal static Task<UserSettingsDto> ReadAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return HttpResponseReader.ReadJsonAsync<UserSettingsDto>(
            response,
            cancellationToken);
    }
}
