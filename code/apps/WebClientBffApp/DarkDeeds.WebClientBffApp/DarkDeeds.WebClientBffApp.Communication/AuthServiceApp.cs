using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DarkDeeds.WebClientBffApp.Communication
{
    public class AuthServiceApp : ServiceAppBase, IAuthServiceApp
    {
        private readonly string _url;

        public AuthServiceApp(IHttpContextAccessor httpContextAccessor,
            IOptions<CommunicationSettings> communicationSettings) : base(httpContextAccessor)
        {
            _url = $"http://{communicationSettings.Value.AuthService}/api/auth";
        }

        public async Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo)
        {
            var url = $"{_url}/signup";
            var response = await (await HttpClient).PostAsync(url, SerializePayload(signUpInfo));
            return await ParseBodyAsync<SignUpResultDto>(response);
        }

        public async Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo)
        {
            var url = $"{_url}/signin";
            var response = await (await HttpClient).PostAsync(url, SerializePayload(signInInfo));
            return await ParseBodyAsync<SignInResultDto>(response);
        }
    }
}
