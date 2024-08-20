namespace DD.Shared.Details.Abstractions.Dto;

public class TaskDto
{
    public int Id { get; set; }

    public DateTime? Date { get; set; }

    public int? Time { get; set; }

    public string Title { get; set; } = string.Empty;

    public int Order { get; set; }

    public bool Completed { get; set; }

    public bool IsProbable { get; set; }

    public bool Deleted { get; set; }

    public TaskTypeDto Type { get; set; }

    public int Version { get; set; }

    public string Uid { get; set; } = string.Empty;
}
