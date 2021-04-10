using AutoMapper;
using DarkDeeds.AuthServiceApp.Contract;
using DarkDeeds.AuthServiceApp.Services.Dto;

namespace DarkDeeds.AuthServiceApp.ContractImpl.Mapping
{
    public class ModelsMappingProfile : Profile
    {
        public ModelsMappingProfile()
        {
            CreateMap<SignInRequest, SignInInfoDto>();
            CreateMap<SignInResultDto, SignInReply>()
                .ForMember(x => x.Token,
                    e => e.MapFrom(x => x.Token ?? string.Empty));
            
            CreateMap<SignUpRequest, SignUpInfoDto>();
            CreateMap<SignUpResultDto, SignInReply>()
                .ForMember(x => x.Token,
                    e => e.MapFrom(x => x.Token ?? string.Empty));
        }
    }
}