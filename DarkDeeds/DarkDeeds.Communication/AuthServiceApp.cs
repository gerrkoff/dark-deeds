using System.Threading.Tasks;
using DarkDeeds.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.Infrastructure.Communication.AuthServiceApp.Dto;

namespace DarkDeeds.Communication
{
    public class AuthServiceApp : ServiceAppBase, IAuthServiceApp
    {
        private const string Url = "http://localhost:5002/api/auth";
        
        public async Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo)
        {
            var url = $"{Url}/signup";
            var response = await HttpClient.PostAsync(url, SerializePayload(signUpInfo));
            return await ParseBodyAsync<SignUpResultDto>(response);
        }

        public async Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo)
        {
            var url = $"{Url}/signin";
            var response = await HttpClient.PostAsync(url, SerializePayload(signInInfo));
            return await ParseBodyAsync<SignInResultDto>(response);
        }
    }
}