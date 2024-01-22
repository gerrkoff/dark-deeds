using DD.TaskService.Domain.Entities.Abstractions;
using DD.TaskService.Domain.Entities.Enums;

namespace DD.TaskService.Domain.Entities;

public class TaskEntity : Entity, IUserOwnedEntity
{
    public string Title { get; set; }
    public int Order { get; set; }
    public DateTime? Date { get; set; }
    public TaskTypeEnum Type { get; set; }
    public int? Time { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsProbable { get; set; }
    public string UserId { get; set; }
}
