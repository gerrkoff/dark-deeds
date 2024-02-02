using AutoMapper;
using DD.Shared.Auth;

namespace DD.ServiceAuth.Details.Web;

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
