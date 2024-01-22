namespace DD.TaskService.Domain.Entities.Abstractions;

public abstract class Entity
{
    public string Uid { get; set; }
    public bool IsDeleted { get; set; }
    public int Version { get; set; }
}
