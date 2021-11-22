using System;
using DarkDeeds.TaskServiceApp.Services.Specifications;
using Moq;

namespace DarkDeeds.TaskServiceApp.Tests.Mocks
{
    public static partial class MocksCreator
    {
        public static Mock<ITaskSpecification> TaskSpec()
        {
            var taskSpecMock = new Mock<ITaskSpecification>();
            taskSpecMock.Setup(x => x.FilterUserOwned(It.IsAny<string>())).Returns(taskSpecMock.Object);
            taskSpecMock.Setup(x => x.FilterNotDeleted()).Returns(taskSpecMock.Object);
            taskSpecMock.Setup(x => x.FilterActual(It.IsAny<DateTime>())).Returns(taskSpecMock.Object);
            taskSpecMock.Setup(x => x.FilterDateInterval(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(taskSpecMock.Object);
            return taskSpecMock;
        }
        
        public static Mock<IPlannedRecurrenceSpecification> PlannedRecurrenceSpec()
        {
            var plannedRecurrenceSpecMock = new Mock<IPlannedRecurrenceSpecification>();
            plannedRecurrenceSpecMock.Setup(x => x.FilterUserOwned(It.IsAny<string>())).Returns(plannedRecurrenceSpecMock.Object);
            plannedRecurrenceSpecMock.Setup(x => x.FilterNotDeleted()).Returns(plannedRecurrenceSpecMock.Object);
            return plannedRecurrenceSpecMock;
        }
    }
}