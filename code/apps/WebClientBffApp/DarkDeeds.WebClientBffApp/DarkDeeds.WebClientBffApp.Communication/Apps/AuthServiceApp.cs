using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.AuthServiceApp.Contract;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto;

namespace DarkDeeds.WebClientBffApp.Communication.Apps
{
    public class AuthServiceApp : IAuthServiceApp
    {
        private readonly IMapper _mapper;
        private readonly AuthService.AuthServiceClient _authServiceClient;

        public AuthServiceApp(IMapper mapper, AuthService.AuthServiceClient authServiceClient)
        {
            _mapper = mapper;
            _authServiceClient = authServiceClient;
        }

        public async Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo)
        {
            var request = _mapper.Map<SignUpRequest>(signUpInfo);
            var reply = await _authServiceClient.SignUpAsync(request);
            return _mapper.Map<SignUpResultDto>(reply);
        }

        public async Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo)
        {
            var request = _mapper.Map<SignInRequest>(signInInfo);
            var reply = await _authServiceClient.SignInAsync(request);
            return _mapper.Map<SignInResultDto>(reply);
        }
    }
}