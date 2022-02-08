using AutoMapper;
using DarkDeeds.ServiceAuth.Contract;
using DarkDeeds.WebClientBff.Infrastructure.Communication.AuthServiceApp.Dto;

namespace DarkDeeds.WebClientBff.Communication.Mapping
{
    class AuthServiceModelsMapping : Profile
    {
        public AuthServiceModelsMapping()
        {
            CreateMap<SignInInfoDto, SignInRequest>();
            CreateMap<SignInReply, SignInResultDto>();
            
            CreateMap<SignUpInfoDto, SignUpRequest>();
            CreateMap<SignUpReply, SignUpResultDto>();
        }
    }
}