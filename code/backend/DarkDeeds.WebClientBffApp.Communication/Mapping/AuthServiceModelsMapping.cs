using AutoMapper;
using DarkDeeds.ServiceAuth.Contract;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto;

namespace DarkDeeds.WebClientBffApp.Communication.Mapping
{
    public class AuthServiceModelsMapping : Profile
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