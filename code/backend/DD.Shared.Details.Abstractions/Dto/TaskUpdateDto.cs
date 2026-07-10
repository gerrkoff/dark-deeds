using System.ComponentModel;

namespace DD.Shared.Details.Abstractions.Dto;

public class TaskUpdateDto
{
    [Description("Globally unique identifier (GUID string) of the task to reorder.")]
    public string Uid { get; set; } = string.Empty;

    [Description("New position of the task within its day.")]
    public int Order { get; set; }
}
