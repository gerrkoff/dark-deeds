using System;
using System.Collections.Generic;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Entities.Models.Base;

namespace DarkDeeds.TaskServiceApp.Entities.Models
{
    public class PlannedRecurrenceEntity : IUserOwnedEntity
    {
        // TODO!
        public int Id { get; set; }
        public string Uid { get; set; }
        public string Task { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EveryNthDay { get; set; }
        public string EveryMonthDay { get; set; }
        public RecurrenceWeekdayEnum? EveryWeekday { get; set; }
        public string UserId { get; set; }
        public bool IsDeleted { get; set; }
        public List<RecurrenceEntity> Recurrences { get; set; } = new List<RecurrenceEntity>();
    }
}