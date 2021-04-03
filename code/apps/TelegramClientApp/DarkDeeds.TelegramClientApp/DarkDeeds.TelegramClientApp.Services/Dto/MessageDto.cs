using Newtonsoft.Json;

namespace DarkDeeds.TelegramClientApp.Services.Dto
{
    public class MessageDto
    {
        [JsonProperty(PropertyName = "message_id")]
        public int MessageId { get; set; }

        [JsonProperty(PropertyName = "from")]
        public UserDto From { get; set; }

        [JsonProperty(PropertyName = "date")]
        public int Date { get; set; }

        [JsonProperty(PropertyName = "chat")]
        public ChatDto Chat { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
}