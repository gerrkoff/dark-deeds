namespace DD.TaskService.Domain.Dto;

public class TaskUpdatedDto
{
    public ICollection<TaskDto> Tasks { get; set; }
    public string UserId { get; set; }
}
