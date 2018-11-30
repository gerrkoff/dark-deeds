namespace DarkDeeds.Data.Entity.Base
{
    public class DeletableEntity : BaseEntity
    {
        public bool IsDeleted { get; set; }
    }
}