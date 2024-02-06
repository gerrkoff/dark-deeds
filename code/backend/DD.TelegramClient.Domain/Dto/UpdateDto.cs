using System.Text.Json.Serialization;

namespace DD.TelegramClient.Domain.Dto;

public class UpdateDto
{
    [JsonPropertyName("update_id")]
    public int UpdateId { get; set; }

    [JsonPropertyName("message")]
    public MessageDto? Message { get; set; }
}
