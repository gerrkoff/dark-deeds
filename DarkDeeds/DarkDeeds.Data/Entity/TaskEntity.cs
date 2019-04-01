using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DarkDeeds.Data.Entity.Base;
using DarkDeeds.Enums;

namespace DarkDeeds.Data.Entity
{
    public class TaskEntity : DeletableEntity
    {
        public string Title { get; set; }
        public int Order { get; set; }
        
        public DateTime? DateTime { get; set; }
        public TaskTimeTypeEnum TimeType { get; set; }
        
        public bool IsCompleted { get; set; }
        public bool IsProbable { get; set; }
        
        [NotMapped]
        public int ClientId { get; set; }

        [Required]
        public string UserId { get; set; }
        
        [Required]
        public UserEntity User { get; set; }
    }
}