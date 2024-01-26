using AutoMapper;
using DarkDeeds.Authentication.Core.Models;
using DarkDeeds.WebClientBff.UseCases.Dto;

namespace DarkDeeds.WebClientBff.UseCases.Mapping
{
    class ModelsMapping : Profile
    {
        public ModelsMapping()
        {
            CreateMap<AuthToken, CurrentUserDto>()
                .ForMember(x => x.Username,
                    e => e.MapFrom(x => x.DisplayName))
                .ForMember(x => x.UserAuthenticated,
                    e => e.MapFrom(x => !string.IsNullOrEmpty(x.Username)));
        }
    }
}