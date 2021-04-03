using AutoMapper;
using DarkDeeds.WebClientBffApp.Entities;
using DarkDeeds.WebClientBffApp.Services.Dto;

namespace DarkDeeds.WebClientBffApp.Services.Mapping
{
    public class ModelsMappingProfile : Profile
    {
        public ModelsMappingProfile()
        {
            CreateMap<SettingsEntity, SettingsDto>().ReverseMap();
        }
    }
}