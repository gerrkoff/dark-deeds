using System;
using System.Threading.Tasks;
using DarkDeeds.Data.Entity;
using DarkDeeds.Enums;
using DarkDeeds.Services.Implementation;
using DarkDeeds.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.Services.RecurrenceCreatorServiceTests
{
    public partial class RecurrenceCreatorServiceTest : BaseTest
    {
        [Fact]
        public async Task CreateAsync_DoNothingIfNoNonDeletedRecurrences()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var plannedRecurrenceRepo = Helper.CreateRepoMock<PlannedRecurrenceEntity>();
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            
            var service = new RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, null, null);
            
            await service.CreateAsync();
            
            plannedRecurrenceRepo.Verify(x => x.GetAll());
            taskRepo.VerifyNoOtherCalls();
            plannedRecurrenceRepo.VerifyNoOtherCalls();
            recurrenceRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateAsync_CreateTaskForRecurrence()
        {
            var someDate = DateTime.Now.Date;
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var plannedRecurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                Id = 1, Task = "Task", EveryNthDay = 1, StartDate = someDate, UserId = "userId"
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(someDate);
            
            var service = new RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null);
            
            await service.CreateAsync();
            
            taskRepo.Verify(x => x.SaveAsync(It.Is<TaskEntity>(
                y => y.Title == "Task" && y.UserId == "userId" && y.Date == someDate
                )));
        }
        
        [Fact]
        public async Task CreateAsync_CreateRecurrenceTaskForRecurrence()
        {
            var now = new DateTime(2019, 10, 10);
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            taskRepo
                .Setup(x => x.SaveAsync(It.IsAny<TaskEntity>()))
                .Returns(Task.CompletedTask)
                .Callback<TaskEntity>(x => x.Id = 23);
            var plannedRecurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                Id = 42, Task = "Task", EveryNthDay = 1, StartDate = now
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(now);
            
            var service = new RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null);
            
            await service.CreateAsync();
            
            recurrenceRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.PlannedRecurrenceId == 42 && y.TaskId == 23 && y.DateTime == now
            )));
        }
        
        [Fact]
        public async Task CreateAsync_CreateEntitiesExpectedNumberOfTimes()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var plannedRecurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                EveryNthDay = 1, StartDate = new DateTime(2019, 9, 3)
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 3));
            
            var service = new RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null);
            
            await service.CreateAsync();
            
            taskRepo.Verify(x => x.SaveAsync(It.IsAny<TaskEntity>()), Times.Exactly(13));
            recurrenceRepo.Verify(x => x.SaveAsync(It.IsAny<RecurrenceEntity>()), Times.Exactly(13));
        }
        
        [Fact]
        public async Task CreateAsync_SimpleTestWeekday()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var plannedRecurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Wednesday
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            
            var service = new RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null);
            
            await service.CreateAsync();
            
            recurrenceRepo.Verify(x => x.GetAll());
            recurrenceRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 9)
            )));
            recurrenceRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 11)
            )));
            recurrenceRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateAsync_SimpleTestMonthDay()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var plannedRecurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryMonthDay = "9,11"
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            
            var service = new RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null);
            
            await service.CreateAsync();
            
            recurrenceRepo.Verify(x => x.GetAll());
            recurrenceRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 9)
            )));
            recurrenceRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 11)
            )));
            recurrenceRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateAsync_SimpleTestNthDay()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var plannedRecurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryNthDay = 6
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            
            var service = new RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null);
            
            await service.CreateAsync();
            
            recurrenceRepo.Verify(x => x.GetAll());
            recurrenceRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 6)
            )));
            recurrenceRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 12)
            )));
            recurrenceRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateAsync_WeekdayAndMonthDayAndNthDayAtTheSameTime()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var plannedRecurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryNthDay = 6, EveryMonthDay = "6,12,13",
                EveryWeekday = RecurrenceWeekdayEnum.Thursday | RecurrenceWeekdayEnum.Wednesday
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            
            var service = new RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null);
            
            await service.CreateAsync();
            
            recurrenceRepo.Verify(x => x.GetAll());
            recurrenceRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 12)
            )));
            recurrenceRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateAsync_IgnoreAlreadyExistingRecurrences()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var plannedRecurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                EveryNthDay = 1, StartDate = new DateTime(2019, 9, 3), Id = 42
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>(new RecurrenceEntity
            {
                PlannedRecurrenceId = 42, DateTime = new DateTime(2019, 9, 3)
            });
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 3));
            
            var service = new RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null);
            
            await service.CreateAsync();
            
            taskRepo.Verify(x => x.SaveAsync(It.IsAny<TaskEntity>()), Times.Exactly(12));
            recurrenceRepo.Verify(x => x.SaveAsync(It.IsAny<RecurrenceEntity>()), Times.Exactly(12));
        }
    }
}