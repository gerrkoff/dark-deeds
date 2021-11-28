namespace DarkDeeds.MongoMigrator.PostgreDal.Models.Base
{
    public class DeletableEntity : BaseEntity
    {
        public bool IsDeleted { get; set; }
    }
}