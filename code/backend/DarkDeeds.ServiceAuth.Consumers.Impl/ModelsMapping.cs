using AutoMapper;
using DarkDeeds.ServiceAuth.Contract;
using DarkDeeds.ServiceAuth.Dto.Dto;

namespace DarkDeeds.ServiceAuth.Consumers.Impl
{
    class ModelsMapping : Profile
    {
        public ModelsMapping()
        {
            CreateMap<SignInInfoDto, SignInRequest>();
            CreateMap<SignInReply, SignInResultDto>();

            CreateMap<SignUpInfoDto, SignUpRequest>();
            CreateMap<SignUpReply, SignUpResultDto>();
        }
    }
}
