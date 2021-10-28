using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Entities.Models.Base;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using Moq;

namespace DarkDeeds.TaskServiceApp.Tests.Mocks
{
    public static partial class MocksCreator
    {
        public static Mock<ITaskRepository> RepoTask(params TaskEntity[] values)
        {
            var repoMock = new Mock<ITaskRepository>();
            repoMock.Setup(x => x.GetAll()).Returns(values.AsQueryable());
            var result = Task.FromResult(values.ToList() as IList<TaskEntity>);
            repoMock.Setup(x => x.GetBySpecAsync(It.IsAny<ISpecification<TaskEntity>>())).Returns(result);
            repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns((string id) =>
            {
                return Task.FromResult(values.FirstOrDefault(x => x.Uid == id));
            });
            return repoMock;
        }
        
        public static Mock<IRepository<T>> Repo<T>(params T[] values)
            where T : DeletableEntity
        {
            var repoMock = new Mock<IRepository<T>>();
            repoMock.Setup(x => x.GetAll()).Returns(values.AsQueryable());
            var result = Task.FromResult(values.ToList() as IList<T>);
            repoMock.Setup(x => x.GetBySpecAsync(It.IsAny<ISpecification<T>>())).Returns(result);
            return repoMock;
        }
        
        public static Mock<IRepositoryNonDeletable<T>> RepoNonDeletable<T>(params T[] values)
            where T : BaseEntity
        {
            var repoMock = new Mock<IRepositoryNonDeletable<T>>();
            repoMock.Setup(x => x.GetAll()).Returns(values.AsQueryable());
            var result = Task.FromResult(values.ToList() as IList<T>);
            repoMock.Setup(x => x.GetBySpecAsync(It.IsAny<ISpecification<T>>())).Returns(result);
            return repoMock;
        }
    }
}