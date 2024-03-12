using DD.TelegramClient.Domain.Dto;
using DD.TelegramClient.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DD.TelegramClient.Details.Web.Controllers;

[AllowAnonymous]
public class BotController(IBotProcessMessageService botProcessMessageService) : ControllerBase
{
    [HttpPost]
    public Task Process([FromBody] UpdateDto update)
    {
        return botProcessMessageService.ProcessMessageAsync(update);
    }
}
