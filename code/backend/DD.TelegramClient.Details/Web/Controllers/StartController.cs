using DD.Shared.Web;
using DD.TelegramClient.Domain.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace DD.TelegramClient.Details.Web.Controllers;

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
