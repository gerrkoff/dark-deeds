﻿using DD.ServiceTask.Domain.Entities.Enums;

namespace DD.ServiceTask.Domain.Dto;

public class PlannedRecurrenceDto
{
    public string Uid { get; set; } = string.Empty;

    public string Task { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? EveryNthDay { get; set; }

    public string? EveryMonthDay { get; set; }

    public RecurrenceWeekday? EveryWeekday { get; set; }

    public bool IsDeleted { get; set; }
}
