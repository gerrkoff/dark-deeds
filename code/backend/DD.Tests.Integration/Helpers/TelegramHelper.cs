using System.Net.Http.Json;
using System.Text.Json;
using DD.Clients.Details.TelegramClient.Dto;
using DD.TelegramClient.Domain.Dto;
using DD.Tests.Integration.Infrastructure;

namespace DD.Tests.Integration.Helpers;

internal static class TelegramHelper
{
    public const string BotRoute = "api/tlgm/bot/integration-tests";

    private static int _nextChatId = -100000;

    public static int CreateUniqueChatId()
    {
        return Interlocked.Decrement(ref _nextChatId);
    }

    public static async Task<string> CreateStartKeyAsync(
        TestUserClient user,
        CancellationToken cancellationToken = default)
    {
        using var response = await user.HttpClient.PostAsync(
            new Uri("api/tlgm/start?timezoneOffset=0", UriKind.Relative),
            content: null,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TelegramStartDto>(
            cancellationToken);
        if (result is null || string.IsNullOrWhiteSpace(result.Url))
            throw new InvalidOperationException("The Telegram start response did not contain a URL.");

        var startValues = new Uri(result.Url).Query
            .TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Where(pair => pair.StartsWith("start=", StringComparison.Ordinal))
            .Select(pair => Uri.UnescapeDataString(pair["start=".Length..]))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToArray();

        return startValues.Length == 1
            ? startValues[0]
            : throw new InvalidOperationException("The Telegram start URL did not contain one start key.");
    }

    public static UpdateDto CreateUpdate(int chatId, string text)
    {
        return new UpdateDto
        {
            UpdateId = Random.Shared.Next(),
            Message = new MessageDto
            {
                Chat = new ChatDto { Id = chatId },
                Text = text,
            },
        };
    }

    public static async Task SendCommandAsync(
        HttpClient client,
        int chatId,
        string text,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.PostAsJsonAsync(
            BotRoute,
            CreateUpdate(chatId, text),
            new JsonSerializerOptions(JsonSerializerDefaults.Web),
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public static Task<string> ReadMessageAsync(
        RecordingBotSendMessageService recorder,
        int chatId,
        TimeSpan timeout)
    {
        return recorder.ReadAsync(chatId, timeout);
    }
}
