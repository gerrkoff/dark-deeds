using AutoMapper;
using DD.ServiceTask.Domain.Dto;
using DD.ServiceTask.Domain.Entities;
using DD.Shared.Details.Abstractions.Dto;

namespace DD.ServiceTask.Domain.Mapping;

public class ModelsMappingProfile : Profile
{
    public ModelsMappingProfile()
    {
        CreateMap<TaskEntity, TaskDto>()
            .ForMember(
                x => x.Completed,
                e => e.MapFrom(x => x.IsCompleted))
            .ForMember(
                x => x.Deleted,
                e => e.MapFrom(x => x.DeletedAt.HasValue));

        CreateMap<TaskDto, TaskEntity>()
            .ForMember(
                x => x.IsCompleted,
                e => e.MapFrom(x => x.Completed));

        CreateMap<PlannedRecurrenceEntity, PlannedRecurrenceDto>().ReverseMap();
    }
}
