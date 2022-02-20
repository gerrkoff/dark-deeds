using AutoMapper;
using DarkDeeds.ServiceTask.Dto;
using DarkDeeds.ServiceTask.Entities;

namespace DarkDeeds.ServiceTask.Services.Mapping
{
    public class ModelsMappingProfile : Profile
    {
        public ModelsMappingProfile()
        {
            CreateMap<TaskEntity, TaskDto>()
                .ForMember(x => x.Completed, e => e.MapFrom(x => x.IsCompleted))
                .ForMember(x => x.Deleted, e => e.MapFrom(x => x.IsDeleted));

            CreateMap<TaskDto, TaskEntity>()
                .ForMember(x => x.IsCompleted, e => e.MapFrom(x => x.Completed))
                .ForMember(x => x.IsDeleted, e => e.MapFrom(x => x.Deleted));

            CreateMap<PlannedRecurrenceEntity, PlannedRecurrenceDto>().ReverseMap();
        }
    }
}
