using System;
using System.Threading.Tasks;
using DarkDeeds.Entities.Enums;
using DarkDeeds.Entities.Models;
using DarkDeeds.Infrastructure.Data;
using DarkDeeds.Models.Dto;
using DarkDeeds.Services.Implementation;
using DarkDeeds.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.Services.RecurrenceServiceTests
{
    public partial class RecurrenceServiceTest : BaseTest
    {
        [Fact]
        public async Task SaveAsync_CheckIsUserCanEdit()
        {
            var permissionMock = new Mock<IPermissionsService>();
            var repoMock = Helper.CreateRepoMock<PlannedRecurrenceEntity>();
            var service = new RecurrenceService(repoMock.Object, permissionMock.Object);

            var list = new PlannedRecurrenceDto[0];
            var userId = "userid";
            await service.SaveAsync(list, userId);
            
            permissionMock.Verify(x => x.CheckIfUserCanEditEntitiesAsync(list, repoMock.Object, userId, It.IsAny<string>()));
        }
        
        [Fact]
        public async Task SaveAsync_DeleteIfDtoIsDeleted()
        {
            var permissionMock = new Mock<IPermissionsService>();
            var repoMock = Helper.CreateRepoMock<PlannedRecurrenceEntity>();
            var service = new RecurrenceService(repoMock.Object, permissionMock.Object);

            await service.SaveAsync(new[] {new PlannedRecurrenceDto {Id = 42, IsDeleted = true}}, null);

            repoMock.Verify(x => x.DeleteAsync(42));
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveAsync_CreateNewIfDtoIdIsLessThan0()
        {
            var permissionMock = new Mock<IPermissionsService>();
            var repoMock = Helper.CreateRepoMock<PlannedRecurrenceEntity>();
            var service = new RecurrenceService(repoMock.Object, permissionMock.Object);

            await service.SaveAsync(new[] {new PlannedRecurrenceDto {Id = -42, Task = "42"}}, "userid1");

            repoMock.Verify(x => x.SaveAsync(It.Is<PlannedRecurrenceEntity>(y =>
                y.Id == 0 &&
                y.Task == "42" &&
                y.UserId == "userid1")));
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveAsync_UpdateIfDtoIdIsMoreThan0()
        {
            var permissionMock = new Mock<IPermissionsService>();
            var repoMock = new Mock<IRepository<PlannedRecurrenceEntity>>();
            repoMock.Setup(x => x.GetByIdAsync(42))
                .Returns(() => Task.FromResult(new PlannedRecurrenceEntity {Id = 42}));
            var service = new RecurrenceService(repoMock.Object, permissionMock.Object);

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
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveAsync_DoNotUpdateIfNoChanges()
        {
            var permissionMock = new Mock<IPermissionsService>();
            var repoMock = new Mock<IRepository<PlannedRecurrenceEntity>>();
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
            var service = new RecurrenceService(repoMock.Object, permissionMock.Object);

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
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveAsync_ReturnUpdatedCount()
        {
            var permissionMock = new Mock<IPermissionsService>();
            var repoMock = new Mock<IRepository<PlannedRecurrenceEntity>>();
            repoMock.Setup(x => x.GetByIdAsync(42))
                .Returns(() => Task.FromResult(new PlannedRecurrenceEntity {Id = 42}));
            repoMock.Setup(x => x.GetByIdAsync(43))
                .Returns(() => Task.FromResult(new PlannedRecurrenceEntity {Id = 43, Task = "Old"}));
            var service = new RecurrenceService(repoMock.Object, permissionMock.Object);

            var result = await service.SaveAsync(new[]
            {
                new PlannedRecurrenceDto {Id = 42},
                new PlannedRecurrenceDto {Id = 43, Task = "New"},
            }, null);

            Assert.Equal(1, result);
        }
    }
}