﻿using System.Threading.Tasks;
using DarkDeeds.Authentication.Core;
using DarkDeeds.TelegramClient.Web.Dto;
using DD.TelegramClient.Domain.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.TelegramClient.Web.Controllers
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
