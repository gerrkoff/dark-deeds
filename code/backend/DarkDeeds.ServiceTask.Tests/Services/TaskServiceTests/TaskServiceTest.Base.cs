using DarkDeeds.ServiceTask.Entities;
using DarkDeeds.ServiceTask.Infrastructure.Data.EntityRepository;
using DarkDeeds.ServiceTask.Infrastructure.Services;
using DarkDeeds.ServiceTask.Services.Implementation;
using DarkDeeds.ServiceTask.Services.Interface;
using DarkDeeds.ServiceTask.Services.Specifications;
using DarkDeeds.ServiceTask.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace DarkDeeds.ServiceTask.Tests.Services.TaskServiceTests
{
    public partial class TaskServiceTest : BaseTest
    {
        private Mock<ITaskRepository> _repoMock;
        private Mock<ILogger<TaskService>> _loggerMock;
        private TaskService _service;
        private Mock<INotifierService> _notifierServiceMock;
        private Mock<ISpecificationFactory> _specFactoryMock;
        private Mock<ITaskSpecification> _taskSpecMock;

        private void CreateService(params TaskEntity[] values)
        {
            _repoMock = MocksCreator.RepoTask(values);
            _loggerMock = new Mock<ILogger<TaskService>>();
            _notifierServiceMock = new Mock<INotifierService>();
            _taskSpecMock = MocksCreator.TaskSpec();
            _specFactoryMock = new Mock<ISpecificationFactory>();
            _specFactoryMock.Setup(x => x.New<ITaskSpecification, TaskEntity>()).Returns(_taskSpecMock.Object);
            _service = new TaskService(
                _repoMock.Object,
                _loggerMock.Object,
                Mapper,
                _notifierServiceMock.Object,
                _specFactoryMock.Object);
        }
    }
}
