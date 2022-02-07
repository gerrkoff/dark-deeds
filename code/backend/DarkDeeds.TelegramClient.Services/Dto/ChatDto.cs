using System.Text.Json.Serialization;

namespace DarkDeeds.TelegramClient.Services.Dto
{
    public class ChatDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        // private, group, supergroup, channel
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
        
        [JsonPropertyName("username")]
        public string UserName { get; set; }
    }
}