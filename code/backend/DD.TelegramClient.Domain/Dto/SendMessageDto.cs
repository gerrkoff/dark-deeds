using System.Text.Json.Serialization;

namespace DD.TelegramClient.Domain.Dto;

public class SendMessageDto
{
    [JsonPropertyName("chat_id")]
    public int ChatId { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

}
