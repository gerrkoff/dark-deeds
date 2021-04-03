using Newtonsoft.Json;

namespace DarkDeeds.TelegramClientApp.Services.Dto
{
    public class ChatDto
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        
        // private, group, supergroup, channel
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        
        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }
        
        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }
        
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }
    }
}