﻿namespace DarkDeeds.ServiceTask.Entities.Models.Abstractions
{
    public abstract class Entity
    {
        public string Uid { get; set; }
        public bool IsDeleted { get; set; }
        public int Version { get; set; }
    }
}