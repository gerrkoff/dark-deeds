using DD.ServiceTask.Domain.Dto;
using DD.ServiceTask.Domain.Entities;

namespace DD.ServiceTask.Domain.Mapping;

public static class PlannedRecurrenceMappingExtensions
{
    public static PlannedRecurrenceDto ToDto(this PlannedRecurrenceEntity entity)
    {
        return new()
        {
            Uid = entity.Uid,
            Task = entity.Task,
            StartDate = DateTime.SpecifyKind(entity.StartDate, DateTimeKind.Utc),
            EndDate = entity.EndDate.HasValue ? DateTime.SpecifyKind(entity.EndDate.Value, DateTimeKind.Utc) : null,
            EveryNthDay = entity.EveryNthDay,
            EveryMonthDay = entity.EveryMonthDay,
            EveryWeekday = entity.EveryWeekday,
        };
    }

    public static PlannedRecurrenceEntity ToEntity(this PlannedRecurrenceDto dto)
    {
        return new()
        {
            Uid = dto.Uid,
            Task = dto.Task,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            EveryNthDay = dto.EveryNthDay,
            EveryMonthDay = dto.EveryMonthDay,
            EveryWeekday = dto.EveryWeekday,
        };
    }
}
