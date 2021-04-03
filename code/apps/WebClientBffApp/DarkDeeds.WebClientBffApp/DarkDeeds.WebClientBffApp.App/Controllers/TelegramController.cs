using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.App.Controllers.Base;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TelegramClientApp.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBffApp.App.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TelegramController : BaseController
    {
        private readonly ITelegramClientApp _telegramClientApp;

        public TelegramController(ITelegramClientApp telegramClientApp)
        {
            _telegramClientApp = telegramClientApp;
        }

        [HttpGet]
        public Task<TelegramStartDto> Start(int timezoneOffset)
        {
            return _telegramClientApp.Start(timezoneOffset);
        }
    }
}