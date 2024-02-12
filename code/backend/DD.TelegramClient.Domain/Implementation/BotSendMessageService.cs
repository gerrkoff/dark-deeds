using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using DD.TelegramClient.Domain.Dto;
using DD.TelegramClient.Domain.Exceptions;

namespace DD.TelegramClient.Domain.Implementation;

public interface IBotSendMessageService
{
    Task SendUnknownCommandAsync(int userChatId);

    Task SendTextAsync(int userChatId, string text);

    Task SendFailedAsync(int userChatId);
}

internal class BotSendMessageService : IBotSendMessageService
{
    private readonly string _apiSendMessage = "https://api.telegram.org/bot{0}/sendMessage";

    public BotSendMessageService(string botToken)
    {
        _apiSendMessage = string.Format(CultureInfo.InvariantCulture, _apiSendMessage, botToken);
    }

    public Task SendUnknownCommandAsync(int userChatId)
    {
        return SendMessage(new SendMessageDto
        {
            ChatId = userChatId,
            Text = "Unknown command",
        });
    }

    public Task SendTextAsync(int userChatId, string text)
    {
        return SendMessage(new SendMessageDto
        {
            ChatId = userChatId,
            Text = text,
        });
    }

    public Task SendFailedAsync(int userChatId)
    {
        return SendMessage(new SendMessageDto
        {
            ChatId = userChatId,
            Text = "Failed",
        });
    }

    protected virtual async Task SendMessage(SendMessageDto message)
    {
        var client = new HttpClient();
        var messageSerialized = JsonSerializer.Serialize(message);
        HttpContent content = new StringContent(messageSerialized, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(new Uri(_apiSendMessage), content);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new ServiceException("Bot integration failed. Response: " +
                                       await response.Content.ReadAsStringAsync());
        }
    }
}

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Debug service is internal and should be in the same file")]
internal sealed class BotSendMessageDebugService(string botToken) : BotSendMessageService(botToken)
{
    protected override Task SendMessage(SendMessageDto message)
    {
        Debug.WriteLine(string.Empty);
        Debug.WriteLine("_________________");
        Debug.WriteLine($"Chat Id: {message.ChatId}");
        Debug.WriteLine("Text:");
        Debug.WriteLine(message.Text);
        Debug.WriteLine("_________________");
        Debug.WriteLine(string.Empty);
        return Task.CompletedTask;
    }
}
