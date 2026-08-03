namespace DD.Shared.TaskText;

// Pure, transport-agnostic result of parsing a single task-text line: the date (null for a
// dateless task), time in minutes from midnight (null when unset), title, type and probable flag.
public sealed record ParsedTaskText(
    DateOnly? Date,
    int? Time,
    string Title,
    TaskTextType Type,
    bool IsProbable);
