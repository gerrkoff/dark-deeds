using System;
using DarkDeeds.ServiceTask.Entities.Enums;

namespace DarkDeeds.ServiceTask.Models.Dto
{
    public class PlannedRecurrenceDto
    {
        public string Uid { get; set; }
        
        public string Task { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public int? EveryNthDay { get; set; }
        
        public string EveryMonthDay { get; set; }

        public RecurrenceWeekdayEnum? EveryWeekday { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}