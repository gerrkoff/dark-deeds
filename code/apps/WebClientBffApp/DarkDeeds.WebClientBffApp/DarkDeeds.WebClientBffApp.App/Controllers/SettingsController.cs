using System.Threading.Tasks;
using DarkDeeds.Authentication;
using DarkDeeds.WebClientBffApp.App.Controllers.Base;
using DarkDeeds.WebClientBffApp.Services.Dto;
using DarkDeeds.WebClientBffApp.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.WebClientBffApp.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : BaseController
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        public Task<SettingsDto> Get()
        {
            return _settingsService.LoadAsync(User.ToAuthToken().UserId);
        }
        
        [HttpPost]
        public Task Post([FromBody] SettingsDto settings)
        {
            return _settingsService.SaveAsync(settings, User.ToAuthToken().UserId);
        }
    }
}