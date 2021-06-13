using System.Threading.Tasks;
using DarkDeeds.Communication.Services.Interface;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp.Dto;

namespace DarkDeeds.WebClientBffApp.Communication.Apps
{
    public class TelegramClientApp : ServiceAppBase, ITelegramClientApp
    {
        public TelegramClientApp(IDdHttpClientFactory clientFactory) : base(clientFactory) {}
        
        protected override string AppName => "telegram-client-app";

        public async Task<TelegramStartDto> Start(int timezoneOffset)
        {
            var url = $"/api/start?timezoneOffset={timezoneOffset}";
            var response = await (await HttpClient).PostAsync(url, null);
            return await ParseBodyAsync<TelegramStartDto>(response);
        }
    }
}