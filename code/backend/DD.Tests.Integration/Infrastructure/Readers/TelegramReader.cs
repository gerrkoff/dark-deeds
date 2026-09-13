using DD.Clients.Details.TelegramClient.Dto;
using Microsoft.AspNetCore.WebUtilities;

namespace DD.Tests.Integration.Infrastructure.Readers;

internal static class TelegramReader
{
    internal static async Task<string> ReadStartKeyAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        var result = await HttpResponseReader.ReadJsonAsync<TelegramStartDto>(
            response,
            cancellationToken);
        if (string.IsNullOrWhiteSpace(result.Url))
        {
            throw new InvalidOperationException(
                "The Telegram start response did not contain a URL.");
        }

        var query = QueryHelpers.ParseQuery(new Uri(result.Url).Query);
        if (!query.TryGetValue("start", out var startValues) ||
            startValues.Count != 1 ||
            string.IsNullOrWhiteSpace(startValues[0]))
        {
            throw new InvalidOperationException(
                "The Telegram start URL did not contain one start key.");
        }

        return startValues[0]!;
    }
}
