using Newtonsoft.Json;

namespace DarkDeeds.TelegramClientApp.Services.Dto
{
    public class UpdateDto
    {
        [JsonProperty(PropertyName = "update_id")]
        public int UpdateId { get; set; }
        
        [JsonProperty(PropertyName = "message")]
        public MessageDto Message { get; set; }
    }
}