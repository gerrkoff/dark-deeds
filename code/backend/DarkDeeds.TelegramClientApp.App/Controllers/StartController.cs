using System.Threading.Tasks;
using DarkDeeds.Authentication;
using DarkDeeds.TelegramClientApp.App.Dto;
using DarkDeeds.TelegramClientApp.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.TelegramClientApp.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StartController : ControllerBase
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