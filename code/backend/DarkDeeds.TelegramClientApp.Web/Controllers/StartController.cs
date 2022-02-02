using System.Threading.Tasks;
using DarkDeeds.Authentication.Core;
using DarkDeeds.TelegramClientApp.Services.Interface;
using DarkDeeds.TelegramClientApp.Web.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.TelegramClientApp.Web.Controllers
{
    public class StartController : BaseController
    {
        private readonly ITelegramService _telegramService;

        public StartController(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        [HttpPost]
        public async Task<TelegramStartDto> Post(int timezoneOffset)
        {   
            string chatKey = await _telegramService.GenerateKey(User.ToAuthToken().UserId, timezoneOffset);
            string botName = "darkdeedsbot";
            return new TelegramStartDto
            {
                Url = $"https://telegram.me/{botName}?start={chatKey}"
            };
        }
    }
}