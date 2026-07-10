using System.ComponentModel;

namespace DD.Shared.Details.Abstractions.Dto;

public enum TaskTypeDto
{
    [Description("Default task type for regular todos.")]
    Simple,

    [Description("Secondary or bonus task.")]
    Additional,

    [Description("Regular maintenance or habitual task.")]
    Routine,

    [Description("Task that happens once per week.")]
    Weekly,
}
