using DarkDeeds.ServiceTask.Tests.Mocks;
using DD.TaskService.Domain.Dto;
using DD.TaskService.Domain.Entities;
using DD.TaskService.Domain.Infrastructure;
using DD.TaskService.Domain.Infrastructure.EntityRepository;
using DD.TaskService.Domain.Services;
using DD.TaskService.Domain.Specifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace DarkDeeds.ServiceTask.Tests.Services.RecurrenceCreatorServiceTests;

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
