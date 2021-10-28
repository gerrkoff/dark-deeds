using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Entities.Models.Base;

namespace DarkDeeds.TaskServiceApp.Entities.Models
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
        
        public List<RecurrenceEntity> Recurrences { get; set; }
    }
}