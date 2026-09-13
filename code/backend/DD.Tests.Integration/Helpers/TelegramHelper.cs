using DD.Tests.Integration.Infrastructure;
using DD.Tests.Integration.Infrastructure.Api;
using DD.Tests.Integration.Infrastructure.Clients;
using DD.Tests.Integration.Infrastructure.Readers;

namespace DD.Tests.Integration.Helpers;

internal static class TelegramHelper
{
    private const string Bot = "integration-tests";

    private static int _nextChatId = -100000;

    public static int CreateUniqueChatId()
    {
        return Interlocked.Decrement(ref _nextChatId);
    }

    public static async Task<string> CreateStartKeyAsync(
        TestUserClient user,
        CancellationToken cancellationToken = default)
    {
        using var response = await TelegramApi.StartAsync(
            user.HttpClient,
            cancellationToken);
        return await TelegramReader.ReadStartKeyAsync(response, cancellationToken);
    }

    public static async Task SendCommandAsync(
        HttpClient client,
        int chatId,
        string text,
        CancellationToken cancellationToken = default)
    {
        using var response = await TelegramApi.SendCommandAsync(
            client,
            chatId,
            text,
            Bot,
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
