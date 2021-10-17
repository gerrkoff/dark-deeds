using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.AuthServiceApp.Contract;
using DarkDeeds.Communication.Services.Interface;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto;

namespace DarkDeeds.WebClientBffApp.Communication.Apps
{
    public class AuthServiceApp : IAuthServiceApp
    {
        private readonly IMapper _mapper;
        private readonly IDdGrpcClientFactory<AuthService.AuthServiceClient> _clientFactory;

        public AuthServiceApp(IMapper mapper, IDdGrpcClientFactory<AuthService.AuthServiceClient> clientFactory)
        {
            _mapper = mapper;
            _clientFactory = clientFactory;
        }

        public async Task<SignUpResultDto> SignUpAsync(SignUpInfoDto signUpInfo)
        {
            var request = _mapper.Map<SignUpRequest>(signUpInfo);
            var client = await _clientFactory.Create();
            var reply = await client.SignUpAsync(request);
            return _mapper.Map<SignUpResultDto>(reply);
        }

        public async Task<SignInResultDto> SignInAsync(SignInInfoDto signInInfo)
        {
            var request = _mapper.Map<SignInRequest>(signInInfo);
            var client = await _clientFactory.Create();
            var reply = await client.SignInAsync(request);
            return _mapper.Map<SignInResultDto>(reply);
        }
    }
}