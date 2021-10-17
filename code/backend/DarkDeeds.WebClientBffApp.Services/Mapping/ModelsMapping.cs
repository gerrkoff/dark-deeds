using AutoMapper;
using DarkDeeds.WebClientBffApp.Entities;
using DarkDeeds.WebClientBffApp.Services.Dto;

namespace DarkDeeds.WebClientBffApp.Services.Mapping
{
    public class ModelsMapping : Profile
    {
        public ModelsMapping()
        {
            CreateMap<SettingsEntity, SettingsDto>().ReverseMap();
        }
    }
}