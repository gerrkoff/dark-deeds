using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using DarkDeeds.TaskServiceApp.Infrastructure.Services;
using DarkDeeds.TaskServiceApp.Services.Implementation;
using DarkDeeds.TaskServiceApp.Services.Interface;
using DarkDeeds.TaskServiceApp.Services.Specifications;
using DarkDeeds.TaskServiceApp.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace DarkDeeds.TaskServiceApp.Tests.Services.TaskServiceTests
{
    public partial class TaskServiceTest
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