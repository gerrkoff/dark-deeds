using System;
using DarkDeeds.Entities.Enums;
using DarkDeeds.Services.Dto.Base;

namespace DarkDeeds.Services.Dto
{
    public class PlannedRecurrenceDto : IDtoWithId
    {
        public int Id { get; set; }
        
        public string Task { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public int? EveryNthDay { get; set; }
        
        public string EveryMonthDay { get; set; }

        public RecurrenceWeekdayEnum? EveryWeekday { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}