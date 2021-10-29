namespace DarkDeeds.TaskServiceApp.Entities.Models.Interfaces
{
    public interface IEntity
    {
        string Uid { get; }
        bool IsDeleted { get; }
    }
}