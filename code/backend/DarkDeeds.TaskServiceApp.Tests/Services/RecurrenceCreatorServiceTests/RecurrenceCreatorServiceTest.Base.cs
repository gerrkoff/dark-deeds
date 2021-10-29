using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using DarkDeeds.TaskServiceApp.Infrastructure.Services;
using DarkDeeds.TaskServiceApp.Models.Dto;
using DarkDeeds.TaskServiceApp.Services.Implementation;
using DarkDeeds.TaskServiceApp.Services.Interface;
using DarkDeeds.TaskServiceApp.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace DarkDeeds.TaskServiceApp.Tests.Services.RecurrenceCreatorServiceTests
{
    public partial class RecurrenceCreatorServiceTest : BaseTest
    {
        private Mock<IPlannedRecurrenceRepository> _plannedRecurrenceRepoMock;
        private readonly Mock<ITaskRepository> _taskRepoMock = MocksCreator.RepoTask();
        private readonly Mock<IDateService> _dateServiceMock = new();
        private readonly Mock<ITaskParserService> _taskParserServiceMock = new();
        private readonly Mock<INotifierService> _notifierServiceMock = new();
        private readonly Mock<ILogger<RecurrenceCreatorService>> _loggerMock = new();

        private RecurrenceCreatorService Service(params PlannedRecurrenceEntity[] values)
        {
            _plannedRecurrenceRepoMock = MocksCreator.RepoRecurrence(values);
            _taskParserServiceMock.Setup(x => x.ParseTask(It.IsAny<string>(), It.IsAny<bool>())).Returns(new TaskDto {Title = "Task"});
            return new(
                _taskRepoMock.Object,
                _plannedRecurrenceRepoMock.Object,
                _dateServiceMock.Object,
                _loggerMock.Object,
                _taskParserServiceMock.Object,
                _notifierServiceMock.Object,
                Mapper
            );
        }
    }
}