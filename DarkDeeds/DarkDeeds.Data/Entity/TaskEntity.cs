using System;
using DarkDeeds.Common.Enums;
using DarkDeeds.Data.Entity.Base;

namespace DarkDeeds.Data.Entity
{
    public class TaskEntity : DeletableEntity
    {
        public string Title { get; set; }
        public int Order { get; set; }
        
        public DateTime? DateTime { get; set; }
        public TaskTimeTypeEnum TimeType { get; set; }
        
        public bool IsCompleted { get; set; }
    }
}