using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Services;
using DarkDeeds.TaskServiceApp.Infrastructure.Services.Dto;
using DarkDeeds.TaskServiceApp.Models.Dto;
using DarkDeeds.TaskServiceApp.Services.Interface;
using DarkDeeds.TaskServiceApp.Tests.Mocks;
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
            var taskRepo = MocksCreator.RepoTask();
            var plannedRecurrenceRepo = MocksCreator.RepoRecurrence();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, null, null, null, null, Mapper);
            
            await service.CreateAsync(0, "");
            
            plannedRecurrenceRepo.Verify(x => x.GetAll());
            taskRepo.VerifyNoOtherCalls();
            plannedRecurrenceRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateAsync_CreateTaskForRecurrence()
        {
            var someDate = DateTime.Now.Date;
            var taskRepo = MocksCreator.RepoTask();
            var plannedRecurrenceRepo = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity
            {
                Id = 1, Task = "Task", EveryNthDay = 1, StartDate = someDate, UserId = "userId", Recurrences = new List<RecurrenceEntity>()
            });
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(someDate);
            var taskParser = CreateTaskParser(new TaskDto {Title = "Task"});
            var taskHubMock = new Mock<INotifierService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, dateServiceMock.Object, null, taskParser, taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
            taskRepo.Verify(x => x.SaveAsync(It.Is<TaskEntity>(
                y => y.Title == "Task" && y.UserId == "userId" && y.Date == someDate && y.Uid != null
                )));
        }
        
        [Fact]
        public async Task CreateAsync_CreateRecurrenceTaskForRecurrence()
        {
            var now = new DateTime(2019, 10, 10);
            var taskRepo = MocksCreator.RepoTask();
            taskRepo
                .Setup(x => x.SaveAsync(It.IsAny<TaskEntity>()))
                .Returns(Task.CompletedTask)
                .Callback<TaskEntity>(x => x.Uid = "uid");
            var plannedRecurrenceRepo = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity
            {
                Id = 42, Task = "Task", EveryNthDay = 1, StartDate = now, UserId = "userId", Recurrences = new List<RecurrenceEntity>()
            });
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(now);
            var taskHubMock = new Mock<INotifierService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
            plannedRecurrenceRepo.Verify(x => x.SaveRecurrences(It.Is<PlannedRecurrenceEntity>(
                    y => y.Recurrences.Any(z => z.DateTime == now && z.TaskUid == "uid")
                )));
        }
        
        [Fact]
        public async Task CreateAsync_CreateEntitiesExpectedNumberOfTimes()
        {
            var taskRepo = MocksCreator.RepoTask();
            var plannedRecurrenceRepo = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity
            {
                EveryNthDay = 1, StartDate = new DateTime(2019, 9, 3), UserId = "userId",
                Recurrences = new List<RecurrenceEntity>()
            });
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 3));
            var taskHubMock = new Mock<INotifierService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
            taskRepo.Verify(x => x.SaveAsync(It.IsAny<TaskEntity>()), Times.Exactly(13));
        }
        
        [Fact]
        public async Task CreateAsync_SimpleTestWeekday()
        {
            var taskRepo = MocksCreator.RepoTask();
            var plannedRecurrenceRepo = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Wednesday, UserId = "userId", Recurrences = new List<RecurrenceEntity>()
            });
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            var taskHubMock = new Mock<INotifierService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
            taskRepo.Verify(x => x.SaveAsync(It.Is<TaskEntity>(
                y => y.Date == new DateTime(2019, 9, 9)
            )));
            taskRepo.Verify(x => x.SaveAsync(It.Is<TaskEntity>(
                y => y.Date == new DateTime(2019, 9, 11)
            )));
            taskRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateAsync_SimpleTestMonthDay()
        {
            var taskRepo = MocksCreator.RepoTask();
            var plannedRecurrenceRepo = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryMonthDay = "9,11", UserId = "userId", Recurrences = new List<RecurrenceEntity>()
            });
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            var taskHubMock = new Mock<INotifierService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");

            taskRepo.Verify(x => x.SaveAsync(It.Is<TaskEntity>(
                y => y.Date == new DateTime(2019, 9, 9)
            )));
            taskRepo.Verify(x => x.SaveAsync(It.Is<TaskEntity>(
                y => y.Date == new DateTime(2019, 9, 11)
            )));
            taskRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateAsync_SimpleTestNthDay()
        {
            var taskRepo = MocksCreator.RepoTask();
            var plannedRecurrenceRepo = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryNthDay = 6, UserId = "userId", Recurrences = new List<RecurrenceEntity>()
            });
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            var taskHubMock = new Mock<INotifierService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
            taskRepo.Verify(x => x.SaveAsync(It.Is<TaskEntity>(
                y => y.Date == new DateTime(2019, 9, 6)
            )));
            taskRepo.Verify(x => x.SaveAsync(It.Is<TaskEntity>(
                y => y.Date == new DateTime(2019, 9, 12)
            )));
            taskRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateAsync_WeekdayAndMonthDayAndNthDayAtTheSameTime()
        {
            var taskRepo = MocksCreator.RepoTask();
            var plannedRecurrenceRepo = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryNthDay = 6, EveryMonthDay = "6,12,13",
                EveryWeekday = RecurrenceWeekdayEnum.Thursday | RecurrenceWeekdayEnum.Wednesday, UserId = "userId", Recurrences = new List<RecurrenceEntity>()
            });
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            var taskHubMock = new Mock<INotifierService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
            taskRepo.Verify(x => x.SaveAsync(It.Is<TaskEntity>(
                y => y.Date == new DateTime(2019, 9, 12)
            )));
            taskRepo.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task CreateAsync_IgnoreAlreadyExistingRecurrences()
        {
            var taskRepo = MocksCreator.RepoTask();
            var plannedRecurrenceRepo = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity
            {
                EveryNthDay = 1, StartDate = new DateTime(2019, 9, 3), Id = 42, UserId = "userId",
                Recurrences = new List<RecurrenceEntity>
                {
                    new() {DateTime = new DateTime(2019, 9, 3)}
                }
            });
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 3));
            var taskHubMock = new Mock<INotifierService>();
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");
            
            taskRepo.Verify(x => x.SaveAsync(It.IsAny<TaskEntity>()), Times.Exactly(12));
        }
        
        [Fact]
        public async Task CreateAsync_IgnorePlannedRecurrencesFromOtherUsers()
        {
            var taskRepo = MocksCreator.RepoTask();
            var plannedRecurrenceRepo = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity
            {
                EveryNthDay = 1, StartDate = new DateTime(2019, 9, 3), Id = 42, UserId = "userId", Recurrences = new List<RecurrenceEntity>()
            });
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 3));
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, dateServiceMock.Object, null, null, null, Mapper);
            
            await service.CreateAsync(0, "userId100500");
            
            taskRepo.Verify(x => x.SaveAsync(It.IsAny<TaskEntity>()), Times.Never);
        }
        
        [Fact]
        public async Task CreateAsync_NoRepeats()
        {
            var taskRepo = MocksCreator.RepoTask();
            var plannedRecurrenceRepo = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), UserId = "userId", Recurrences = new List<RecurrenceEntity>()
            });
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, dateServiceMock.Object, null, CreateTaskParser(), null, Mapper);
            
            await service.CreateAsync(0, "userId");
            
            taskRepo.Verify(x => x.SaveAsync(It.IsAny<TaskEntity>()), Times.Never);
        }
        
        [Fact]
        public async Task CreateAsync_NotifyAboutCreatedTasks()
        {
            var taskRepo = MocksCreator.RepoTask();
            var plannedRecurrenceRepo = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity
            {
                StartDate = new DateTime(2019, 9, 6), EveryMonthDay = "6", UserId = "userId", Recurrences = new List<RecurrenceEntity>()
            });
            var dateServiceMock = new Mock<IDateService>();
            dateServiceMock.SetupGet(x => x.Now).Returns(new DateTime(2019, 9, 6));
            var taskHubMock = new Mock<INotifierService>();
            var taskParser = CreateTaskParser(new TaskDto {Title = "Task"});
            
            var service = new TaskServiceApp.Services.Implementation.RecurrenceCreatorService(taskRepo.Object, plannedRecurrenceRepo.Object, dateServiceMock.Object, null, taskParser, taskHubMock.Object, Mapper);
            
            await service.CreateAsync(0, "userId");

            taskHubMock.Verify(x => x.TaskUpdated(
                It.Is<TaskUpdatedDto>(y => y.Tasks.Count == 1 && y.Tasks.First().Title == "Task")
            ));
        }
    }
}