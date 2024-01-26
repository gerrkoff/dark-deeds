using System.Threading.Tasks;
using DarkDeeds.Authentication.Core;
using DD.WebClientBff.Domain.Dto;
using DD.WebClientBff.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBff.Web.Controllers;

public class SettingsController(
    ISettingsService settingsService,
    IHttpContextAccessor httpContextAccessor) : BaseController
{
    [HttpGet]
    public Task<SettingsDto> Get()
    {
        return settingsService.LoadAsync(httpContextAccessor.HttpContext.User.ToAuthToken().UserId);
    }

    [HttpPost]
    public Task Post([FromBody] SettingsDto settings)
    {
        return settingsService.SaveAsync(settings, httpContextAccessor.HttpContext.User.ToAuthToken().UserId);
    }
}
