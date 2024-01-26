using System.Text.Json.Serialization;

namespace DD.TelegramClient.Domain.Dto;

public class MessageDto
{
    [JsonPropertyName("message_id")]
    public int MessageId { get; set; }

    [JsonPropertyName("from")]
    public UserDto From { get; set; }

    [JsonPropertyName("date")]
    public int Date { get; set; }

    [JsonPropertyName("chat")]
    public ChatDto Chat { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}
