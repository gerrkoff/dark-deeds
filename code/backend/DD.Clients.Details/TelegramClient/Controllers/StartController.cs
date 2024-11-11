using DD.Clients.Details.TelegramClient.Dto;
using DD.Shared.Details.Services;
using DD.TelegramClient.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace DD.Clients.Details.TelegramClient.Controllers;

[ApiController]
[Route("api/tlgm/[controller]")]
public class StartController(
    IUserAuth userAuth,
    ITelegramService telegramService)
    : ControllerBase
{
    [HttpPost]
    public async Task<TelegramStartDto> Post(int timezoneOffset)
    {
        var chatKey = await telegramService.GenerateKey(userAuth.UserId(), timezoneOffset);
        var botName = "darkdeedsbot";
        return new TelegramStartDto
        {
            Url = $"https://telegram.me/{botName}?start={chatKey}",
        };
    }
}
