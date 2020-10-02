using AutoMapper;
using DarkDeeds.Entities.Models;
using DarkDeeds.Models.Dto;

namespace DarkDeeds.Models.Mapping
{
    public class ServicesMappingProfile : Profile
    {
        public ServicesMappingProfile()
        {
            CreateMap<TaskEntity, TaskDto>()
                .ForMember(x => x.Completed, e => e.MapFrom(x => x.IsCompleted))
                .ForMember(x => x.Deleted, e => e.MapFrom(x => x.IsDeleted));

            CreateMap<TaskDto, TaskEntity>()
                .ForMember(x => x.IsCompleted, e => e.MapFrom(x => x.Completed))
                .ForMember(x => x.IsDeleted, e => e.MapFrom(x => x.Deleted));

            CreateMap<SettingsEntity, SettingsDto>().ReverseMap();
            
            CreateMap<PlannedRecurrenceEntity, PlannedRecurrenceDto>().ReverseMap();
        }
    }
}