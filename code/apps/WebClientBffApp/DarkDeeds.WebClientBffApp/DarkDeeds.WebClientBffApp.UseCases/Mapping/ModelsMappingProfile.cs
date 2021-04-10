using AutoMapper;
using DarkDeeds.Authentication.Models;
using DarkDeeds.WebClientBffApp.UseCases.Handlers.Account.Dto;

namespace DarkDeeds.WebClientBffApp.UseCases.Mapping
{
    public class ModelsMappingProfile : Profile
    {
        public ModelsMappingProfile()
        {
            CreateMap<AuthToken, CurrentUserDto>()
                .ForMember(x => x.Username,
                    e => e.MapFrom(x => x.DisplayName))
                .ForMember(x => x.UserAuthenticated,
                    e => e.MapFrom(x => !string.IsNullOrEmpty(x.Username)));
        }
    }
}