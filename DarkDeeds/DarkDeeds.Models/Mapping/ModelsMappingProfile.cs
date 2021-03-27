using AutoMapper;
using DarkDeeds.Entities.Models;
using DarkDeeds.Models.Dto;

namespace DarkDeeds.Models.Mapping
{
    public class ModelsMappingProfile : Profile
    {
        public ModelsMappingProfile()
        {
            CreateMap<SettingsEntity, SettingsDto>().ReverseMap();
        }
    }
}