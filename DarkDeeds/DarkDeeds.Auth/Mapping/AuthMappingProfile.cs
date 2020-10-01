using AutoMapper;
using DarkDeeds.Auth.Dto;
using DarkDeeds.Auth.Models;

namespace DarkDeeds.Auth.Mapping
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<CurrentUser, CurrentUserDto>()
                .ForMember(x => x.Username, e => e.MapFrom(x => x.DisplayName))
                .ForMember(x => x.UserAuthenticated, e => e.MapFrom(x => !string.IsNullOrEmpty(x.Username)));
        }
    }
}