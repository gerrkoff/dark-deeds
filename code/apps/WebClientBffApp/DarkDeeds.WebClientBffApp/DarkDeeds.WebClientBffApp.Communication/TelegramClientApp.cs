using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DarkDeeds.WebClientBffApp.Communication
{
    public class TelegramClientApp : ServiceAppBase, ITelegramClientApp
    {
        private readonly string _url;
        
        public TelegramClientApp(IHttpContextAccessor httpContextAccessor,
            IOptions<CommunicationSettings> communicationSettings) : base(httpContextAccessor)
        {
            _url = $"http://{communicationSettings.Value.TelegramClient}/api";
        }

        public async Task<TelegramStartDto> Start(int timezoneOffset)
        {
            var url = $"{_url}/start?timezoneOffset={timezoneOffset}";
            var response = await (await HttpClient).PostAsync(url, null);
            return await ParseBodyAsync<TelegramStartDto>(response);
        }
    }
}