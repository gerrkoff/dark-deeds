using System.Collections.Concurrent;
using System.Threading.Channels;
using DD.TelegramClient.Domain.Services;

namespace DD.Tests.Integration.Infrastructure.ExternalDependencies;

public sealed class TestBotSendMessageService : IBotSendMessageService
{
    private const int ChannelCapacity = 32;
    private readonly ConcurrentDictionary<int, Channel<string>> _messages = new();

    public Task SendUnknownCommandAsync(int userChatId)
    {
        return RecordAsync(userChatId, "Unknown command");
    }

    public Task SendTextAsync(int userChatId, string text)
    {
        return RecordAsync(userChatId, text);
    }

    public Task SendFailedAsync(int userChatId)
    {
        return RecordAsync(userChatId, "Failed");
    }

    public async Task<string> ReadAsync(int userChatId, TimeSpan timeout)
    {
        using var cancellation = new CancellationTokenSource(timeout);

        try
        {
            return await GetChannel(userChatId).Reader.ReadAsync(cancellation.Token);
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"No Telegram message was recorded for chat {userChatId} within {timeout}.");
        }
    }

    private async Task RecordAsync(int userChatId, string message)
    {
        await GetChannel(userChatId).Writer.WriteAsync(message);
    }

    private Channel<string> GetChannel(int userChatId)
    {
        return _messages.GetOrAdd(
            userChatId,
            static _ => Channel.CreateBounded<string>(
                new BoundedChannelOptions(ChannelCapacity)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = false,
                    SingleWriter = false,
                }));
    }
}
