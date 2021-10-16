using System.Text.Json.Serialization;

namespace DarkDeeds.TelegramClientApp.Services.Dto
{
    public class SendMessageDto
    {
        [JsonPropertyName("chat_id")]
        public int ChatId { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

    }
}