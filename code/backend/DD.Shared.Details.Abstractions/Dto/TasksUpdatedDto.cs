using System.ComponentModel;

namespace DD.Shared.Details.Abstractions.Dto;

public class TasksUpdatedDto
{
    [Description("The tasks affected by the update.")]
    public ICollection<TaskDto> Tasks { get; init; } = [];

    [Description("Identifier of the user who owns the tasks.")]
    public string UserId { get; set; } = string.Empty;

    [Description("Identifier of the client that made the change; null for server-originated changes.")]
    public string? ClientId { get; set; }
}
