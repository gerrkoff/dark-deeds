using System;
using System.ComponentModel.DataAnnotations;
using DarkDeeds.Entities.Models.Base;

namespace DarkDeeds.Entities.Models
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