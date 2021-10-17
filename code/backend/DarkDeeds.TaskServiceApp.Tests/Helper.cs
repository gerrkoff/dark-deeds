using System.Linq;
using DarkDeeds.TaskServiceApp.Entities.Models.Base;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using Moq;

namespace DarkDeeds.TaskServiceApp.Tests
{
    public static class Helper
    {
        public static Mock<IRepository<T>> CreateRepoMock<T>(params T[] values)
            where T : DeletableEntity
        {
            var repoMock = new Mock<IRepository<T>>();
            repoMock.Setup(x => x.GetAll()).Returns(values.AsQueryable());
            return repoMock;
        }
        
        public static Mock<IRepositoryNonDeletable<T>> CreateRepoNonDeletableMock<T>(params T[] values)
            where T : BaseEntity
        {
            var repoMock = new Mock<IRepositoryNonDeletable<T>>();
            repoMock.Setup(x => x.GetAll()).Returns(values.AsQueryable());
            return repoMock;
        }
    }
}