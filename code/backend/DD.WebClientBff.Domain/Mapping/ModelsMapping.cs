using AutoMapper;
using DD.WebClientBff.Domain.Dto;
using DD.WebClientBff.Domain.Entities;

namespace DD.WebClientBff.Domain.Mapping;

class ModelsMapping : Profile
{
    public ModelsMapping()
    {
        CreateMap<SettingsEntity, SettingsDto>().ReverseMap();
    }
}
