using AutoMapper;
using DarkDeeds.Entities.Models;
using DarkDeeds.Models;
using DarkDeeds.Models.Account;
using DarkDeeds.Models.Entity;

namespace DarkDeeds.Services.Mapping
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

            CreateMap<CurrentUser, CurrentUserDto>()
                .ForMember(x => x.Username, e => e.MapFrom(x => x.DisplayName))
                .ForMember(x => x.UserAuthenticated, e => e.MapFrom(x => !string.IsNullOrEmpty(x.Username)));

            CreateMap<SettingsEntity, SettingsDto>().ReverseMap();
            
            CreateMap<PlannedRecurrenceEntity, PlannedRecurrenceDto>().ReverseMap();
        }
    }
}