using System;
using System.Threading.Tasks;
using DarkDeeds.Api.Controllers.Base;
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
        public Task<string> Key()
        {
            return _telegramService.GenerateKey(GetUser().UserId);
        }
    }
}