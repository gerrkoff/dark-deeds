using System.Threading.Tasks;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto;
using Microsoft.AspNetCore.Http;

namespace DarkDeeds.WebClientBffApp.Communication
{
    public class AuthServiceApp : ServiceAppBase, IAuthServiceApp
    {
        private const string Url = "http://localhost:5002/api/auth";
        
        public AuthServiceApp(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
        
        public async Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo)
        {
            var url = $"{Url}/signup";
            var response = await (await HttpClient).PostAsync(url, SerializePayload(signUpInfo));
            return await ParseBodyAsync<SignUpResultDto>(response);
        }

        public async Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo)
        {
            var url = $"{Url}/signin";
            var response = await (await HttpClient).PostAsync(url, SerializePayload(signInInfo));
            return await ParseBodyAsync<SignInResultDto>(response);
        }
    }
}