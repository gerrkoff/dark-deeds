namespace DD.ServiceTask.Domain.Dto;

public class TaskUpdatedDto
{
    public ICollection<TaskDto> Tasks { get; init; } = new List<TaskDto>();

    public string UserId { get; set; } = string.Empty;
}
