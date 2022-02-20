using AutoMapper;
using DarkDeeds.ServiceAuth.Contract;
using DarkDeeds.ServiceAuth.Dto.Dto;

namespace DarkDeeds.ServiceAuth.ContractImpl.Mapping
{
    class ModelsMappingProfile : Profile
    {
        public ModelsMappingProfile()
        {
            CreateMap<SignInRequest, SignInInfoDto>();
            CreateMap<SignInResultDto, SignInReply>()
                .ForMember(x => x.Token,
                    e => e.MapFrom(x => x.Token ?? string.Empty));

            CreateMap<SignUpRequest, SignUpInfoDto>();
            CreateMap<SignUpResultDto, SignUpReply>()
                .ForMember(x => x.Token,
                    e => e.MapFrom(x => x.Token ?? string.Empty));
        }
    }
}
