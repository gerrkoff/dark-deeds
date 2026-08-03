using DD.Shared.TaskText;

namespace DD.TerminalClient.Domain.Models;

public enum TerminalTaskType
{
    Simple,
    Additional,
    Routine,
    Weekly,
}

public static class TerminalTaskTypeExtensions
{
    // Maps the transport-agnostic shared-parser type onto the terminal domain type. The two enums
    // are intentionally identical; the mapping is explicit so a future divergence fails to compile.
    public static TerminalTaskType ToTerminalTaskType(this TaskTextType type)
    {
        return type switch
        {
            TaskTextType.Simple => TerminalTaskType.Simple,
            TaskTextType.Additional => TerminalTaskType.Additional,
            TaskTextType.Routine => TerminalTaskType.Routine,
            TaskTextType.Weekly => TerminalTaskType.Weekly,
            _ => TerminalTaskType.Simple,
        };
    }
}
