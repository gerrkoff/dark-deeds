using System.ComponentModel;

namespace DD.Shared.Details.Abstractions.Dto;

public class TaskCreateDto
{
    [Description("Task title. Required, non-empty.")]
    public string Title { get; set; } = string.Empty;

    [Description("Date the task is scheduled for. Optional; omit for a task with no date.")]
    public DateTime? Date { get; set; }

    [Description("Time of day in minutes from midnight (e.g. 17:30 => 1050). Optional.")]
    public int? Time { get; set; }

    [Description("Task type: Simple (default), Additional, Routine or Weekly.")]
    public TaskTypeDto Type { get; set; }

    [Description("Marks a task that might not happen; displayed differently. Defaults to false.")]
    public bool IsProbable { get; set; }
}
