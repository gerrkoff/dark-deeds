using System.Text.Json.Serialization;

namespace DarkDeeds.TelegramClientApp.Services.Dto
{
    public class UserDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
        
        [JsonPropertyName("username")]
        public string UserName { get; set; }
    }
}