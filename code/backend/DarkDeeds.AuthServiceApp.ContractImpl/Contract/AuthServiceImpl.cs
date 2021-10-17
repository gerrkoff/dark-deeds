using System.Threading.Tasks;
using AutoMapper;
using DarkDeeds.AuthServiceApp.Contract;
using DarkDeeds.AuthServiceApp.Services.Dto;
using Grpc.Core;
using DarkDeeds.AuthServiceApp.Services.Services.Interface;

namespace DarkDeeds.AuthServiceApp.ContractImpl.Contract
{
    public class AuthServiceImpl : AuthService.AuthServiceBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthServiceImpl(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        public override async Task<SignInReply> SignIn(SignInRequest request, ServerCallContext context)
        {
            var signInInfo = _mapper.Map<SignInInfoDto>(request);
            SignInResultDto result = await _authService.SignInAsync(signInInfo);
            return _mapper.Map<SignInReply>(result);
        }
        
        public override async Task<SignUpReply> SignUp(SignUpRequest request, ServerCallContext context)
        {
            var signUpInfoDto = _mapper.Map<SignUpInfoDto>(request);
            SignUpResultDto result = await _authService.SignUpAsync(signUpInfoDto);
            return _mapper.Map<SignUpReply>(result);
        }
    }
}