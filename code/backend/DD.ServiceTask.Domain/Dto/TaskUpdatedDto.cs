namespace DD.ServiceTask.Domain.Dto;

public class TaskUpdatedDto
{
    public ICollection<TaskDto> Tasks { get; set; }
    public string UserId { get; set; }
}
