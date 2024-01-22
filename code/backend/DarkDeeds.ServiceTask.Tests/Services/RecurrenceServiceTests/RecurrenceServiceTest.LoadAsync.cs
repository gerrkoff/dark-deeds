using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Tests.Mocks;
using DD.TaskService.Domain.Entities;
using DD.TaskService.Domain.Services;
using DD.TaskService.Domain.Specifications;
using Moq;
using Xunit;

namespace DarkDeeds.ServiceTask.Tests.Services.RecurrenceServiceTests;

public partial class RecurrenceServiceTest
{
    private readonly Mock<ISpecificationFactory> _specFactoryMock = new();
    private readonly Mock<IPlannedRecurrenceSpecification> _plannedRecurrenceSpecMock = MocksCreator.PlannedRecurrenceSpec();

    public RecurrenceServiceTest()
    {
        _specFactoryMock.Setup(x => x.New<IPlannedRecurrenceSpecification, PlannedRecurrenceEntity>())
            .Returns(_plannedRecurrenceSpecMock.Object);
    }

    [Fact]
    public async Task LoadActualTasksAsync_Positive()
    {
        var userId = "userid";

        var repoMock = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity
        {
            StartDate = new DateTime(1, DateTimeKind.Unspecified),
            EndDate = new DateTime(1, DateTimeKind.Unspecified)
        });

        var service = new RecurrenceService(repoMock.Object, Mapper, _specFactoryMock.Object);

        var result = (await service.LoadAsync(userId)).ToList();

        Assert.Equal(DateTimeKind.Utc, result[0].StartDate.Kind);
        Assert.Equal(DateTimeKind.Utc, result[0].EndDate!.Value.Kind);
        _plannedRecurrenceSpecMock.Verify(x => x.FilterUserOwned(userId));
        repoMock.Verify(x => x.GetBySpecAsync(_plannedRecurrenceSpecMock.Object));
    }
}
