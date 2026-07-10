using System.ComponentModel;

namespace DD.Shared.Details.Abstractions.Dto;

public class TaskDto
{
    [Description("Numeric task identifier.")]
    public int Id { get; set; }

    [Description("Date the task is scheduled for; null for a task with no date.")]
    public DateTime? Date { get; set; }

    [Description("Time of day in minutes from midnight (e.g. 17:30 => 1050); null if unset.")]
    public int? Time { get; set; }

    [Description("Task title.")]
    public string Title { get; set; } = string.Empty;

    [Description("Position of the task within its day, used for ordering.")]
    public int Order { get; set; }

    [Description("Whether the task is completed.")]
    public bool Completed { get; set; }

    [Description("Marks a task that might not happen; displayed differently.")]
    public bool IsProbable { get; set; }

    [Description("Whether the task has been soft-deleted.")]
    public bool Deleted { get; set; }

    [Description("Task type: Simple, Additional, Routine or Weekly.")]
    public TaskTypeDto Type { get; set; }

    [Description("Optimistic-concurrency version, incremented on each save.")]
    public int Version { get; set; }

    [Description("Globally unique task identifier (GUID string).")]
    public string Uid { get; set; } = string.Empty;
}
