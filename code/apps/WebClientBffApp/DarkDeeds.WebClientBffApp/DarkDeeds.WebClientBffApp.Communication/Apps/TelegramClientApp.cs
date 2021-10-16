using System.Threading.Tasks;
using DarkDeeds.Communication;
using DarkDeeds.Communication.Services.Interface;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp.Dto;

namespace DarkDeeds.WebClientBffApp.Communication.Apps
{
    public class TelegramClientApp : ITelegramClientApp
    {
        private const string AppName = "telegram-client";
        
        private readonly IDdHttpClientFactory _clientFactory;

        public TelegramClientApp(IDdHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<TelegramStartDto> Start(int timezoneOffset)
        {
            var url = $"/api/start?timezoneOffset={timezoneOffset}";
            var client = await _clientFactory.Create(AppName);
            var response = await client.PostAsync(url, null);
            return await response.DeserializeBodyAsync<TelegramStartDto>();
        }
    }
}