using Newtonsoft.Json;

namespace DarkDeeds.BotIntegration.Dto
{
    public class SendMessageDto
    {
        [JsonProperty(PropertyName = "chat_id")]
        public int ChatId { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

    }
}