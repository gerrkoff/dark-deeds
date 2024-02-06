namespace DD.ServiceTask.Domain.Entities.Abstractions;

public abstract class Entity
{
    public string Uid { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public int Version { get; set; }
}
