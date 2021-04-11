using System;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Services;
using DarkDeeds.TaskServiceApp.Models.Dto;
using DarkDeeds.TaskServiceApp.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Services.RecurrenceCreatorService
{
    public partial class RecurrenceCreatorServiceTest : BaseTest
    {
        private ITaskParserService CreateTaskParser(TaskDto task = null)
        {
            if (task == null)
                task = new TaskDto();
            var taskParser = new Mock<ITaskParserService>();
            taskParser.Setup(x => x.ParseTask(It.IsAny<string>(), It.IsAny<bool>())).Returns(task);
            return taskParser.Object;
        }
        
        [Fact]
        public async Task CreateAsync_DoNothingIfNoNonDeletedRecurrences()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var plannedRecurrenceRepo = Helper.CreateRepoMock<PlannedRecurrenceEntity>();
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, null, null, null, null, Mapper);
            
            await service.CreateAsync(0, "");
            
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
            var taskParser = CreateTaskParser(new TaskDto {Title = "Task"});
            var taskHubMock = new Mock<ITaskHubService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null, taskParser, taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
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
                Id = 42, Task = "Task", EveryNthDay = 1, StartDate = now, UserId = "userId"
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(now);
            var taskHubMock = new Mock<ITaskHubService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
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
                EveryNthDay = 1, StartDate = new DateTime(2019, 9, 3), UserId = "userId"
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 3));
            var taskHubMock = new Mock<ITaskHubService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
            taskRepo.Verify(x => x.SaveAsync(It.IsAny<TaskEntity>()), Times.Exactly(13));
            recurrenceRepo.Verify(x => x.SaveAsync(It.IsAny<RecurrenceEntity>()), Times.Exactly(13));
        }
        
        [Fact]
        public async Task CreateAsync_SimpleTestWeekday()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var plannedRecurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Wednesday, UserId = "userId"
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            var taskHubMock = new Mock<ITaskHubService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
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
                StartDate = new DateTime(2019, 9, 6), EveryMonthDay = "9,11", UserId = "userId"
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            var taskHubMock = new Mock<ITaskHubService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
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
                StartDate = new DateTime(2019, 9, 6), EveryNthDay = 6, UserId = "userId"
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            var taskHubMock = new Mock<ITaskHubService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
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
                EveryWeekday = RecurrenceWeekdayEnum.Thursday | RecurrenceWeekdayEnum.Wednesday, UserId = "userId"
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            var taskHubMock = new Mock<ITaskHubService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
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
                EveryNthDay = 1, StartDate = new DateTime(2019, 9, 3), Id = 42, UserId = "userId"
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock(new RecurrenceEntity
            {
                PlannedRecurrenceId = 42, DateTime = new DateTime(2019, 9, 3)
            });
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 3));
            var taskHubMock = new Mock<ITaskHubService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
            taskRepo.Verify(x => x.SaveAsync(It.IsAny<TaskEntity>()), Times.Exactly(12));
            recurrenceRepo.Verify(x => x.SaveAsync(It.IsAny<RecurrenceEntity>()), Times.Exactly(12));
        }
        
        [Fact]
        public async Task CreateAsync_IgnorePlannedRecurrencesFromOtherUsers()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var plannedRecurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                EveryNthDay = 1, StartDate = new DateTime(2019, 9, 3), Id = 42, UserId = "userId"
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 3));
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null, null, null, Mapper);
            
            await service.CreateAsync(0, "userId100500");
            
            taskRepo.Verify(x => x.SaveAsync(It.IsAny<TaskEntity>()), Times.Never);
            recurrenceRepo.Verify(x => x.SaveAsync(It.IsAny<RecurrenceEntity>()), Times.Never);
        }
        
        [Fact]
        public async Task CreateAsync_NoRepeats()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var plannedRecurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), UserId = "userId"
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), null, Mapper);
            
            await service.CreateAsync(0, "userId");
            
            recurrenceRepo.Verify(x => x.SaveAsync(It.IsAny<RecurrenceEntity>()), Times.Never);
        }
        
        [Fact]
        public async Task CreateAsync_NotifyAboutCreatedTasks()
        {
            var taskRepo = Helper.CreateRepoMock<TaskEntity>();
            var plannedRecurrenceRepo = Helper.CreateRepoMock(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryMonthDay = "6", UserId = "userId"
            });
            var recurrenceRepo = Helper.CreateRepoNonDeletableMock<RecurrenceEntity>();
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            var taskHubMock = new Mock<ITaskHubService>();
            var taskParser = CreateTaskParser(new TaskDto {Title = "Task"});
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, recurrenceRepo.Object, dateServiceMock.Object, null, taskParser, taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");

            taskHubMock.Verify(x => x.Update(
                It.Is<TaskDto[]>(y => y.Length == 1 && y[0].Title == "Task")
            ));
        }
    }
}