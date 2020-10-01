using System;
using System.ComponentModel.DataAnnotations;
using DarkDeeds.Entities.Enums;
using DarkDeeds.Entities.Models.Base;

namespace DarkDeeds.Entities.Models
{
    public class PlannedRecurrenceEntity : DeletableEntity, IUserOwnedEntity
    {
        public string Task { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public int? EveryNthDay { get; set; }
        
        public string EveryMonthDay { get; set; }

        public RecurrenceWeekdayEnum? EveryWeekday { get; set; }

        [Required]
        public string UserId { get; set; }
        
        [Required]
        public UserEntity User { get; set; }
    }
}