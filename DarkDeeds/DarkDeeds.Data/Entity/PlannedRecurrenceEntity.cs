using System;
using System.ComponentModel.DataAnnotations;
using DarkDeeds.Data.Entity.Base;
using DarkDeeds.Enums;

namespace DarkDeeds.Data.Entity
{
    public class PlannedRecurrenceEntity : DeletableEntity
    {
        public string Task { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public int EveryNthDay { get; set; }
        
        public string EveryMonthDay { get; set; }

        public RecurrenceWeekdayEnum? EveryWeekday { get; set; }

        [Required]
        public string UserId { get; set; }
        
        [Required]
        public UserEntity User { get; set; }
    }
}