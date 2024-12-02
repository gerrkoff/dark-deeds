namespace DD.ServiceTask.Domain.Entities.Abstractions;

public abstract class Entity
{
    public string Uid { get; set; } = string.Empty;

    public DateTimeOffset? DeletedAt { get; set; }

    public int Version { get; set; }
}
