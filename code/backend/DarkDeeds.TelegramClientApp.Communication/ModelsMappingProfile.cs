using System;
using AutoMapper;
using DarkDeeds.TaskServiceApp.Contract;
using DarkDeeds.TelegramClientApp.Infrastructure.Communication.TaskServiceApp.Dto;

namespace DarkDeeds.TelegramClientApp.Communication
{
    public class ModelsMappingProfile : Profile
    {
        public ModelsMappingProfile()
        {
            CreateMap<TaskDto, TaskModel>()
                .ForMember(x => x.Date, e =>
                    e.MapFrom(x => x.Date.HasValue ? x.Date.Value.Ticks : 0))
                .ForMember(x => x.DateExist, e =>
                    e.MapFrom(x => x.Date.HasValue))
                .ForMember(x => x.Time, e =>
                    e.MapFrom(x => x.Time ?? 0))
                .ForMember(x => x.TimeExist, e =>
                    e.MapFrom(x => x.Time.HasValue))
                .ForMember(x => x.Probable,
                    e => e.MapFrom(x => x.IsProbable));

            CreateMap<TaskModel, TaskDto>()
                .ForMember(x => x.Date, e =>
                    e.MapFrom(x => x.DateExist ? (DateTime?) new DateTime(x.Date, DateTimeKind.Utc) : null))
                .ForMember(x => x.Time, e =>
                    e.MapFrom(x => x.TimeExist ? (int?) x.Time : null))
                .ForMember(x => x.IsProbable,
                    e => e.MapFrom(x => x.Probable));
        }
    }
}