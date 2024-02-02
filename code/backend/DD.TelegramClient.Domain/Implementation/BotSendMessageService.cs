using System.Diagnostics;
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

class BotSendMessageService : IBotSendMessageService
{
    private readonly string _apiSendMessage = "https://api.telegram.org/bot{0}/sendMessage";

    public BotSendMessageService(string botToken)
    {
        _apiSendMessage = string.Format(_apiSendMessage, botToken);
    }

    public Task SendUnknownCommandAsync(int userChatId) => SendMessage(new SendMessageDto
    {
        ChatId = userChatId,
        Text = "Unknown command"
    });

    public Task SendTextAsync(int userChatId, string text) => SendMessage(new SendMessageDto
    {
        ChatId = userChatId,
        Text = text
    });

    public Task SendFailedAsync(int userChatId) => SendMessage(new SendMessageDto
    {
        ChatId = userChatId,
        Text = "Failed"
    });

    protected virtual async Task SendMessage(SendMessageDto message)
    {
        var client = new HttpClient();
        string messageSerialized = JsonSerializer.Serialize(message);
        HttpContent content = new StringContent(messageSerialized, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(new Uri(_apiSendMessage), content);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new ServiceException("Bot integration failed. Response: " +
                                       await response.Content.ReadAsStringAsync());
    }
}

class BotSendMessageDebugService(string botToken) : BotSendMessageService(botToken)
{
    protected override Task SendMessage(SendMessageDto message)
    {
        Debug.WriteLine("");
        Debug.WriteLine("_________________");
        Debug.WriteLine($"Chat Id: {message.ChatId}");
        Debug.WriteLine("Text:");
        Debug.WriteLine(message.Text);
        Debug.WriteLine("_________________");
        Debug.WriteLine("");
        return Task.CompletedTask;
    }
}
