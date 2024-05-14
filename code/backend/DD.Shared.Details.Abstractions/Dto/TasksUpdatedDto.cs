namespace DD.Shared.Details.Abstractions.Dto;

public class TasksUpdatedDto
{
    public ICollection<TaskDto> Tasks { get; init; } = new List<TaskDto>();

    public string UserId { get; set; } = string.Empty;
}
