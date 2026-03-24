using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Entities.Enums;
using DD.Shared.Details.Abstractions.Dto;

namespace DD.ServiceTask.Domain.Mapping;

public static class TaskMappingExtensions
{
    public static TaskDto ToDto(this TaskEntity entity)
    {
        return new()
        {
            Uid = entity.Uid,
            Date = entity.Date,
            Time = entity.Time,
            Title = entity.Title,
            Order = entity.Order,
            Completed = entity.IsCompleted,
            IsProbable = entity.IsProbable,
            Deleted = entity.DeletedAt.HasValue,
            Type = (TaskTypeDto)entity.Type,
            Version = entity.Version,
        };
    }

    public static TaskEntity ToEntity(this TaskDto dto)
    {
        return new()
        {
            Uid = dto.Uid,
            Date = dto.Date,
            Time = dto.Time,
            Title = dto.Title,
            Order = dto.Order,
            IsCompleted = dto.Completed,
            IsProbable = dto.IsProbable,
            Type = (TaskType)dto.Type,
            Version = dto.Version,
        };
    }

    public static void ApplyTo(this TaskDto dto, TaskEntity entity)
    {
        entity.Uid = dto.Uid;
        entity.Date = dto.Date;
        entity.Time = dto.Time;
        entity.Title = dto.Title;
        entity.Order = dto.Order;
        entity.IsCompleted = dto.Completed;
        entity.IsProbable = dto.IsProbable;
        entity.Type = (TaskType)dto.Type;
        entity.Version = dto.Version;
    }
}
