using System;
using System.Threading.Tasks;
using DarkDeeds.TaskServiceApp.Entities.Enums;
using DarkDeeds.TaskServiceApp.Entities.Models;
using DarkDeeds.TaskServiceApp.Infrastructure.Data;
using DarkDeeds.TaskServiceApp.Models.Dto;
using DarkDeeds.TaskServiceApp.Models.Exceptions;
using DarkDeeds.TaskServiceApp.Services.Interface;
using DarkDeeds.TaskServiceApp.Tests.Mocks;
using Moq;
using Xunit;

namespace DarkDeeds.TaskServiceApp.Tests.Services.RecurrenceService
{
    public partial class RecurrenceServiceTest : BaseTest
    {
        [Fact]
        public async Task SaveAsync_CheckIsUserCanEdit()
        {
            var repoMock = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity {UserId = "other", Id = 1});
            var service = new TaskServiceApp.Services.Implementation.RecurrenceService(repoMock.Object, Mapper);

            var list = new PlannedRecurrenceDto[] {new() {Id = 1}};
            var userId = "userid";

            await Assert.ThrowsAsync<ServiceException>(() => service.SaveAsync(list, userId));
        }
        
        [Fact]
        public async Task SaveAsync_DeleteIfDtoIsDeleted()
        {
            var permissionMock = new Mock<IPermissionsService>();
            var repoMock = MocksCreator.RepoRecurrence();
            var service = new TaskServiceApp.Services.Implementation.RecurrenceService(repoMock.Object, Mapper);

            await service.SaveAsync(new[] {new PlannedRecurrenceDto {Id = 42, IsDeleted = true}}, null);

            repoMock.Verify(x => x.DeleteAsync(42));
            repoMock.Verify(x => x.GetAll());
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveAsync_CreateNewIfDtoIdIsLessThan0()
        {
            var permissionMock = new Mock<IPermissionsService>();
            var repoMock = MocksCreator.RepoRecurrence();
            var service = new TaskServiceApp.Services.Implementation.RecurrenceService(repoMock.Object, Mapper);

            await service.SaveAsync(new[] {new PlannedRecurrenceDto {Id = -42, Task = "42"}}, "userid1");

            repoMock.Verify(x => x.SaveAsync(It.Is<PlannedRecurrenceEntity>(y =>
                y.Task == "42" &&
                y.UserId == "userid1")));
            repoMock.Verify(x => x.GetAll());
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveAsync_UpdateIfDtoIdIsMoreThan0()
        {
            var permissionMock = new Mock<IPermissionsService>();
            var repoMock = new Mock<IPlannedRecurrenceRepository>();
            repoMock.Setup(x => x.GetByIdAsync(42))
                .Returns(() => Task.FromResult(new PlannedRecurrenceEntity {Id = 42}));
            var service = new TaskServiceApp.Services.Implementation.RecurrenceService(repoMock.Object, Mapper);

            await service.SaveAsync(new[]
            {
                new PlannedRecurrenceDto
                {
                    Id = 42,
                    Task = "42",
                    StartDate = new DateTime(2010, 10, 10),
                    EndDate = new DateTime(2011, 11, 11),
                    EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Sunday,
                    EveryMonthDay = "1,2,3",
                    EveryNthDay = 100500
                }
            }, null);

            repoMock.Verify(x => x.SaveAsync(It.Is<PlannedRecurrenceEntity>(y =>
                y.Id == 42 &&
                y.Task == "42" &&
                y.StartDate == new DateTime(2010, 10, 10) &&
                y.EndDate == new DateTime(2011, 11, 11) &&
                y.EveryWeekday == (RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Sunday) &&
                y.EveryMonthDay == "1,2,3" &&
                y.EveryNthDay == 100500)));
            repoMock.Verify(x => x.GetByIdAsync(42));
            repoMock.Verify(x => x.GetAll());
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveAsync_DoNotUpdateIfNoChanges()
        {
            var permissionMock = new Mock<IPermissionsService>();
            var repoMock = new Mock<IPlannedRecurrenceRepository>();
            repoMock.Setup(x => x.GetByIdAsync(42))
                .Returns(() => Task.FromResult(new PlannedRecurrenceEntity
                {
                    Id = 42,
                    Task = "42",
                    StartDate = new DateTime(2010, 10, 10),
                    EndDate = new DateTime(2011, 11, 11),
                    EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Sunday,
                    EveryMonthDay = "1,2,3",
                    EveryNthDay = 100500
                }));
            var service = new TaskServiceApp.Services.Implementation.RecurrenceService(repoMock.Object, Mapper);

            await service.SaveAsync(new[]
            {
                new PlannedRecurrenceDto
                {
                    Id = 42,
                    Task = "42",
                    StartDate = new DateTime(2010, 10, 10),
                    EndDate = new DateTime(2011, 11, 11),
                    EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Sunday,
                    EveryMonthDay = "1,2,3",
                    EveryNthDay = 100500
                }
            }, null);
            
            repoMock.Verify(x => x.GetByIdAsync(42));
            repoMock.Verify(x => x.GetAll());
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveAsync_ReturnUpdatedCount()
        {
            var permissionMock = new Mock<IPermissionsService>();
            var repoMock = new Mock<IPlannedRecurrenceRepository>();
            repoMock.Setup(x => x.GetByIdAsync(42))
                .Returns(() => Task.FromResult(new PlannedRecurrenceEntity {Id = 42}));
            repoMock.Setup(x => x.GetByIdAsync(43))
                .Returns(() => Task.FromResult(new PlannedRecurrenceEntity {Id = 43, Task = "Old"}));
            var service = new TaskServiceApp.Services.Implementation.RecurrenceService(repoMock.Object, Mapper);

            var result = await service.SaveAsync(new[]
            {
                new PlannedRecurrenceDto {Id = 42},
                new PlannedRecurrenceDto {Id = 43, Task = "New"},
            }, null);

            Assert.Equal(1, result);
        }
    }
}