using System.Threading.Tasks;
using DarkDeeds.Api.Controllers.Base;
using DarkDeeds.Models;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TelegramController : BaseUserController
    {
        private readonly ITelegramService _telegramService;

        public TelegramController(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        [HttpGet]
        public async Task<TelegramStartDto> Start()
        {
            string chatKey = await _telegramService.GenerateKey(GetUser().UserId);
            string botName = "darkdeedsbot";
            return new TelegramStartDto
            {
                Url = $"https://telegram.me/{botName}?start={chatKey}"
            };
        }
    }
}