﻿using DD.ServiceTask.Domain.Entities.Enums;

namespace DD.ServiceTask.Domain.Dto;

public class TaskDto
{
    public int Id { get; set; }
    public DateTime? Date { get; set; }
    public int? Time { get; set; }
    public string Title { get; set; }
    public int Order { get; set; }
    public bool Completed { get; set; }
    public bool IsProbable { get; set; }
    public bool Deleted { get; set; }
    public TaskTypeEnum Type { get; set; }
    public int Version { get; set; }
    public string Uid { get; set; }
}
