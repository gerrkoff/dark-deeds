using System;
using System.ComponentModel.DataAnnotations;
using DarkDeeds.Data.Entity.Base;

namespace DarkDeeds.Data.Entity
{
    public class RecurrenceTaskEntity : BaseEntity
    {
        public DateTime DateTime { get; set; }
        
        [Required]
        public int RecurrenceId { get; set; }
        
        [Required]
        public int TaskId { get; set; }
        
        [Required]
        public RecurrenceEntity Recurrence { get; set; }
        
        [Required]
        public TaskEntity Task { get; set; }
    }
}