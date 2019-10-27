using System;
using System.Threading.Tasks;
using DarkDeeds.Data.Entity;
using DarkDeeds.Enums;
using DarkDeeds.Services.Implementation;
using DarkDeeds.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public partial class RecurrenceCreatorServiceTest : BaseTest
    {
        [Fact]
        public async Task CreateRecurrenceTasks_DoNothingIfNoNonDeletedRecurrences()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var recurrenceRepo = Helper.CreateRepoMock<PlannedRecurrenceEntity>();
            var recurrenceTaskRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            
            var service = new RecurrenceCreatorService(taskRepo.Object, recurrenceRepo.Object, recurrenceTaskRepo.Object, null);
            
            await service.CreateAsync();
            
            recurrenceRepo.Verify(x => x.GetAll());
            taskRepo.VerifyNoOtherCalls();
            recurrenceRepo.VerifyNoOtherCalls();
            recurrenceTaskRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateRecurrenceTasks_CreateTaskForRecurrence()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var recurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                Id = 1, Task = "Task", EveryNthDay = 1, StartDate = DateTime.Now, UserId = "userId"
            });
            var recurrenceTaskRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(DateTime.Now);
            
            var service = new RecurrenceCreatorService(taskRepo.Object, recurrenceRepo.Object, recurrenceTaskRepo.Object, dateServiceMock.Object);
            
            await service.CreateAsync();
            
            taskRepo.Verify(x => x.SaveAsync(It.Is<TaskEntity>(
                y => y.Title == "Task" && y.UserId == "userId"
                )));
        }
        
        [Fact]
        public async Task CreateRecurrenceTasks_CreateRecurrenceTaskForRecurrence()
        {
            var now = new DateTime(2019, 10, 10);
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            taskRepo
                .Setup(x => x.SaveAsync(It.IsAny<TaskEntity>()))
                .Returns(Task.CompletedTask)
                .Callback<TaskEntity>(x => x.Id = 23);
            var recurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                Id = 42, Task = "Task", EveryNthDay = 1, StartDate = now
            });
            var recurrenceTaskRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(now);
            
            var service = new RecurrenceCreatorService(taskRepo.Object, recurrenceRepo.Object, recurrenceTaskRepo.Object, dateServiceMock.Object);
            
            await service.CreateAsync();
            
            recurrenceTaskRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.PlannedRecurrenceId == 42 && y.TaskId == 23 && y.DateTime == now
            )));
        }
        
        [Fact]
        public async Task CreateRecurrenceTasks_CreateEntitiesExpectedNumberOfTimes()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var recurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                EveryNthDay = 1, StartDate = new DateTime(2019, 9, 3)
            });
            var recurrenceTaskRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 3));
            
            var service = new RecurrenceCreatorService(taskRepo.Object, recurrenceRepo.Object, recurrenceTaskRepo.Object, dateServiceMock.Object);
            
            await service.CreateAsync();
            
            taskRepo.Verify(x => x.SaveAsync(It.IsAny<TaskEntity>()), Times.Exactly(13));
            recurrenceTaskRepo.Verify(x => x.SaveAsync(It.IsAny<RecurrenceEntity>()), Times.Exactly(13));
        }
        
        [Fact]
        public async Task CreateRecurrenceTasks_SimpleTestWeekdays()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var recurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Wednesday
            });
            var recurrenceTaskRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            
            var service = new RecurrenceCreatorService(taskRepo.Object, recurrenceRepo.Object, recurrenceTaskRepo.Object, dateServiceMock.Object);
            
            await service.CreateAsync();
            
            recurrenceTaskRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 9)
            )));
            recurrenceTaskRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 11)
            )));
            recurrenceTaskRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateRecurrenceTasks_SimpleTestMonthDays()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var recurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryMonthDay = "9,11"
            });
            var recurrenceTaskRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            
            var service = new RecurrenceCreatorService(taskRepo.Object, recurrenceRepo.Object, recurrenceTaskRepo.Object, dateServiceMock.Object);
            
            await service.CreateAsync();
            
            recurrenceTaskRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 9)
            )));
            recurrenceTaskRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 11)
            )));
            recurrenceTaskRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateRecurrenceTasks_SimpleTestEveryNumberOfDays()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var recurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryNthDay = 6
            });
            var recurrenceTaskRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            
            var service = new RecurrenceCreatorService(taskRepo.Object, recurrenceRepo.Object, recurrenceTaskRepo.Object, dateServiceMock.Object);
            
            await service.CreateAsync();
            
            recurrenceTaskRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 6)
            )));
            recurrenceTaskRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 12)
            )));
            recurrenceTaskRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateRecurrenceTasks_WeekdayAndMonthDayAndEveryNumberOfDaysAtTheSameTime()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var recurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryNthDay = 6, EveryMonthDay = "6,12,13",
                EveryWeekday = RecurrenceWeekdayEnum.Thursday | RecurrenceWeekdayEnum.Wednesday
            });
            var recurrenceTaskRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            
            var service = new RecurrenceCreatorService(taskRepo.Object, recurrenceRepo.Object, recurrenceTaskRepo.Object, dateServiceMock.Object);
            
            await service.CreateAsync();
            
            recurrenceTaskRepo.Verify(x => x.SaveAsync(It.Is<RecurrenceEntity>(
                y => y.DateTime == new DateTime(2019, 9, 12)
            )));
            recurrenceTaskRepo.VerifyNoOtherCalls();
        }
    }
}