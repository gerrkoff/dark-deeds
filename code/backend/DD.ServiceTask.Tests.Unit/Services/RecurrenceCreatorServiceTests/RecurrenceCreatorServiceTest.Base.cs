using DD.ServiceTask.Domain.Dto;
using DD.ServiceTask.Domain.Entities;
using DD.ServiceTask.Domain.Infrastructure;
using DD.ServiceTask.Domain.Infrastructure.EntityRepository;
using DD.ServiceTask.Domain.Services;
using DD.ServiceTask.Domain.Specifications;
using Microsoft.Extensions.Logging;
using Moq;
using MocksCreator = DD.ServiceTask.Tests.Unit.Mocks.MocksCreator;

namespace DD.ServiceTask.Tests.Unit.Services.RecurrenceCreatorServiceTests;

public partial class RecurrenceCreatorServiceTest : BaseTest
{
    private Mock<IPlannedRecurrenceRepository> _plannedRecurrenceRepoMock;
    private readonly Mock<ITaskRepository> _taskRepoMock = MocksCreator.RepoTask();
    private readonly Mock<IDateService> _dateServiceMock = new();
    private readonly Mock<ITaskParserService> _taskParserServiceMock = new();
    private readonly Mock<INotifierService> _notifierServiceMock = new();
    private readonly Mock<ILogger<RecurrenceCreatorService>> _loggerMock = new();
    private Mock<ISpecificationFactory> _specFactoryMock;
    private Mock<IPlannedRecurrenceSpecification> _plannedRecurrenceSpecMock;

    private RecurrenceCreatorService Service(params PlannedRecurrenceEntity[] values)
    {
        _plannedRecurrenceSpecMock = MocksCreator.PlannedRecurrenceSpec();
        _specFactoryMock = new Mock<ISpecificationFactory>();
        _specFactoryMock.Setup(x => x.New<IPlannedRecurrenceSpecification, PlannedRecurrenceEntity>()).Returns(_plannedRecurrenceSpecMock.Object);

        _plannedRecurrenceRepoMock = MocksCreator.RepoRecurrence(values);
        _taskParserServiceMock.Setup(x => x.ParseTask(It.IsAny<string>(), It.IsAny<bool>())).Returns(new TaskDto {Title = "Task"});
        return new(
            _taskRepoMock.Object,
            _plannedRecurrenceRepoMock.Object,
            _dateServiceMock.Object,
            _loggerMock.Object,
            _taskParserServiceMock.Object,
            _notifierServiceMock.Object,
            Mapper,
            _specFactoryMock.Object
        );
    }
}
