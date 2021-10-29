using System;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Entities.Models.Interfaces;

namespace DarkDeeds.TaskServiceApp.Entities.Models
{
    public class TaskEntity : IUserOwnedEntity
    {
        public string Uid { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public DateTime? Date { get; set; }
        public TaskTypeEnum Type { get; set; }
        public int? Time { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsProbable { get; set; }
        public int Version { get; set; }
        public bool IsDeleted { get; set; }
        public string UserId { get; set; }
    }
}