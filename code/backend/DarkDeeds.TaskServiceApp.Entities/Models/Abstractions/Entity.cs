namespace DarkDeeds.TaskServiceApp.Entities.Models.Abstractions
{
    public abstract class Entity
    {
        public string Uid { get; set; }
        public bool IsDeleted { get; set; }
    }
}