using AutoMapper;
using DarkDeeds.Backend.Entities;
using DarkDeeds.WebClientBff.Services.Dto;

namespace DarkDeeds.WebClientBff.Services.Mapping
{
    public class ModelsMapping : Profile
    {
        public ModelsMapping()
        {
            CreateMap<SettingsEntity, SettingsDto>().ReverseMap();
        }
    }
}