using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp.Dto;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.WebClientBffApp.Communication
{
    public class TelegramClientApp : ServiceAppBase, ITelegramClientApp
    {
        private const string Url = "http://localhost:5003/api";
        
        public TelegramClientApp(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        public async Task<TelegramStartDto> Start(int timezoneOffset)
        {
            var url = $"{Url}/start?timezoneOffset={timezoneOffset}";
            var response = await (await HttpClient).PostAsync(url, null);
            return await ParseBodyAsync<TelegramStartDto>(response);
        }
    }
}