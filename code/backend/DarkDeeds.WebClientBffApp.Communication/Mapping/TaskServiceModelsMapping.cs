using System;
using AutoMapper;
using DarkDeeds.TaskServiceApp.Contract;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Dto;
using DarkDeeds.WebClientBffApp.Infrastructure.Communication.TaskServiceApp.Enums;

namespace DarkDeeds.WebClientBffApp.Communication.Mapping
{
    public class TaskServiceModelsMapping : Profile
    {
        public TaskServiceModelsMapping()
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

            CreateMap<PlannedRecurrenceDto, PlannedRecurrenceModel>()
                .ForMember(x => x.EndDate, e =>
                    e.MapFrom(x => x.EndDate.HasValue ? x.EndDate.Value.Ticks : 0))
                .ForMember(x => x.EndDateExist, e =>
                    e.MapFrom(x => x.EndDate.HasValue))
                .ForMember(x => x.EveryNthDay, e =>
                    e.MapFrom(x => x.EveryNthDay ?? 0))
                .ForMember(x => x.EveryNthDayExist, e =>
                    e.MapFrom(x => x.EveryNthDay.HasValue))
                .ForMember(x => x.EveryWeekday, e =>
                    e.MapFrom(x => x.EveryWeekday ?? 0))
                .ForMember(x => x.EveryWeekdayExist, e =>
                    e.MapFrom(x => x.EveryWeekday.HasValue))
                .ForMember(x => x.StartDate, e =>
                    e.MapFrom(x => x.StartDate.Ticks))
                .ForMember(x => x.EveryMonthDay, e =>
                    e.MapFrom(x => x.EveryMonthDay ?? string.Empty));
            
            CreateMap<PlannedRecurrenceModel, PlannedRecurrenceDto>()
                .ForMember(x => x.EndDate, e =>
                    e.MapFrom(x => x.EndDateExist ? (DateTime?) new DateTime(x.EndDate, DateTimeKind.Utc) : null))
                .ForMember(x => x.EveryNthDay, e =>
                    e.MapFrom(x => x.EveryNthDayExist ? (int?) x.EveryNthDay : null))
                .ForMember(x => x.EveryWeekday, e =>
                    e.MapFrom(x => x.EveryWeekdayExist ? (RecurrenceWeekdayEnum?) x.EveryWeekday : null))
                .ForMember(x => x.EveryMonthDay, e =>
                    e.MapFrom(x => x.EveryMonthDay != string.Empty ? x.EveryMonthDay : null))
                .ForMember(x => x.StartDate, e =>
                    e.MapFrom(x => new DateTime(x.StartDate, DateTimeKind.Utc)));
        }
    }
}