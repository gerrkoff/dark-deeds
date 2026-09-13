using System.Net.Http.Json;
using DD.TelegramClient.Domain.Dto;

namespace DD.Tests.Integration.Infrastructure.Api;

internal static class TelegramApi
{
    private static readonly Uri StartUri =
        new("api/tlgm/start?timezoneOffset=0", UriKind.Relative);

    internal static Task<HttpResponseMessage> StartAsync(
        HttpClient client,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsync(StartUri, content: null, cancellationToken);
    }

    internal static Task<HttpResponseMessage> SendCommandAsync(
        HttpClient client,
        int chatId,
        string text,
        string bot,
        CancellationToken cancellationToken = default)
    {
        var uri = new Uri($"api/tlgm/bot/{Uri.EscapeDataString(bot)}", UriKind.Relative);
        return client.PostAsJsonAsync(
            uri,
            new UpdateDto
            {
                UpdateId = Random.Shared.Next(),
                Message = new MessageDto
                {
                    Chat = new ChatDto { Id = chatId },
                    Text = text,
                },
            },
            cancellationToken);
    }
}
