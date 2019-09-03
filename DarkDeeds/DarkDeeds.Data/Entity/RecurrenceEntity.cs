using System;
using System.ComponentModel.DataAnnotations;
using DarkDeeds.Data.Entity.Base;
using DarkDeeds.Enums;

namespace DarkDeeds.Data.Entity
{
    public class RecurrenceEntity : DeletableEntity
    {
        public DateTime StartDate { get; set; }
        
        public string Task { get; set; }
        
        public string MonthDays { get; set; }

        public RecurrenceWeekdayEnum? Weekdays { get; set; }

        public int EveryNumberOfDays { get; set; }

        [Required]
        public string UserId { get; set; }
        
        [Required]
        public UserEntity User { get; set; }
    }
}