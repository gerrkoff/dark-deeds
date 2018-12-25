using System.Linq;
using DarkDeeds.Data.Entity.Base;
using DarkDeeds.Data.Repository;
using Moq;

namespace DarkDeeds.Tests
{
    public static class Helper
    {
        public static Mock<IRepository<T>> CreateRepoMock<T>(params T[] values)
            where T : BaseEntity
        {
            var repoMock = new Mock<IRepository<T>>();
            repoMock.Setup(x => x.GetAll()).Returns(values.AsQueryable());
            return repoMock;
        }
    }
}