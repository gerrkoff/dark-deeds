namespace DarkDeeds.Entities.Models.Base
{
    public class DeletableEntity : BaseEntity
    {
        public bool IsDeleted { get; set; }
    }
}