using System.Threading.Tasks;
using DarkDeeds.Api.Controllers.Base;
using DarkDeeds.Authentication;
using DarkDeeds.Models.Dto;
using DarkDeeds.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DarkDeeds.Api.Controllers
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