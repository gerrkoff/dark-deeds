using AutoMapper;
using DarkDeeds.Data.Entity;
using DarkDeeds.Models;

namespace DarkDeeds.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaskEntity, TaskDto>()
                .ForMember(x => x.Completed, e => e.MapFrom(x => x.IsCompleted))
                .ForMember(x => x.Deleted, e => e.MapFrom(x => x.IsDeleted));

            CreateMap<TaskDto, TaskEntity>()
                .ForMember(x => x.IsCompleted, e => e.MapFrom(x => x.Completed))
                .ForMember(x => x.IsDeleted, e => e.MapFrom(x => x.Deleted));
        }
    }
}