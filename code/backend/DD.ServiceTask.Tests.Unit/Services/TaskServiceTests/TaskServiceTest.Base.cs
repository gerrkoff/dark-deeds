using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Infrastructure;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;
using DD.ServiceTask.Domain.Services;
using DD.ServiceTask.Domain.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using MocksCreator = DD.ServiceTask.Tests.Unit.Mocks.MocksCreator;

namespace DD.ServiceTask.Tests.Unit.Services.TaskServiceTests;

public partial class TaskServiceTest : BaseTest
{
    private Mock<ITaskRepository> _repoMock = new();
    private Mock<ILogger<TaskService>> _loggerMock = new();
    private Mock<INotifierService> _notifierServiceMock = new();
    private Mock<ISpecificationFactory> _specFactoryMock = new();
    private Mock<ITaskSpecification> _taskSpecMock = new();

    private TaskService CreateService(params TaskEntity[] values)
    {
        _repoMock = MocksCreator.RepoTask(values);
        _loggerMock = new Mock<ILogger<TaskService>>();
        _notifierServiceMock = new Mock<INotifierService>();
        _taskSpecMock = MocksCreator.TaskSpec();
        _specFactoryMock = new Mock<ISpecificationFactory>();
        _specFactoryMock.Setup(x => x.New<ITaskSpecification, TaskEntity>()).Returns(_taskSpecMock.Object);
        return new TaskService(
            _repoMock.Object,
            _loggerMock.Object,
            Mapper,
            _notifierServiceMock.Object,
            _specFactoryMock.Object);
    }
}
