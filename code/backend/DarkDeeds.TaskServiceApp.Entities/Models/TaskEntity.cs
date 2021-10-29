using System;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Entities.Models.Abstractions;

namespace DarkDeeds.TaskServiceApp.Entities.Models
{
    public class TaskEntity : Entity, IUserOwnedEntity
    {
        public string Title { get; set; }
        public int Order { get; set; }
        public DateTime? Date { get; set; }
        public TaskTypeEnum Type { get; set; }
        public int? Time { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsProbable { get; set; }
        public int Version { get; set; }
        public string UserId { get; set; }
    }
}