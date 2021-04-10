using AutoMapper;
using DarkDeeds.AuthServiceApp.Contract;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.AuthServiceApp.Dto;

namespace DarkDeeds.WebClientBffApp.Communication.Mapping
{
    public class ModelsMappingProfile : Profile
    {
        public ModelsMappingProfile()
        {
            CreateMap<SignInInfoDto, SignInRequest>();
            CreateMap<SignInReply, SignInResultDto>();
            
            CreateMap<SignUpInfoDto, SignUpRequest>();
            CreateMap<SignUpReply, SignUpResultDto>();
        }
    }
}