namespace DD.Shared.Details.Abstractions.Dto;

public class TasksUpdatedDto
{
    public ICollection<TaskDto> Tasks { get; init; } = [];

    public string UserId { get; set; } = string.Empty;

    public string? ClientId { get; set; }
}
