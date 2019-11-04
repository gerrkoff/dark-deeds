using System;
using DarkDeeds.Enums;

namespace DarkDeeds.Models
{
    public class PlannedRecurrenceDto
    {
        public int Id { get; set; }
        
        public string Task { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public int? EveryNthDay { get; set; }
        
        public string EveryMonthDay { get; set; }

        public RecurrenceWeekdayEnum? EveryWeekday { get; set; }
    }
}