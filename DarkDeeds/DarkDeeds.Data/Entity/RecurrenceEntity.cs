using System;
using System.ComponentModel.DataAnnotations;
using DarkDeeds.Data.Entity.Base;

namespace DarkDeeds.Data.Entity
{
    public class RecurrenceEntity : BaseEntity
    {
        public DateTime DateTime { get; set; }
        
        [Required]
        public int PlannedRecurrenceId { get; set; }
        
        [Required]
        public int TaskId { get; set; }
        
        [Required]
        public PlannedRecurrenceEntity PlannedRecurrence { get; set; }
        
        [Required]
        public TaskEntity Task { get; set; }
    }
}