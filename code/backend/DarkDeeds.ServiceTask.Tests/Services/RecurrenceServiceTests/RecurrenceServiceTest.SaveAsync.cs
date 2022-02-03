using System;
using System.Threading.Tasks;
using DarkDeeds.ServiceTask.Entities.Enums;
using DarkDeeds.ServiceTask.Entities.Models;
using DarkDeeds.ServiceTask.Infrastructure.Data.EntityRepository;
using DarkDeeds.ServiceTask.Models.Dto;
using DarkDeeds.ServiceTask.Models.Exceptions;
using DarkDeeds.ServiceTask.Services.Implementation;
using DarkDeeds.ServiceTask.Services.Specifications;
using DarkDeeds.ServiceTask.Tests.Mocks;
using Moq;
using Xunit;

namespace DarkDeeds.ServiceTask.Tests.Services.RecurrenceServiceTests
{
    public partial class RecurrenceServiceTest : BaseTest
    {
        [Fact]
        public async Task SaveAsync_CheckIsUserCanEdit()
        {
            var repoMock = MocksCreator.RepoRecurrence(new PlannedRecurrenceEntity {UserId = "other", Uid = "1"});
            repoMock.Setup(x => x.AnyAsync(It.IsAny<IPlannedRecurrenceSpecification>()))
                .Returns(Task.FromResult(true));
            
            var service = new RecurrenceService(repoMock.Object, Mapper, _specFactoryMock.Object);

            var list = new PlannedRecurrenceDto[] {new() {Uid = "1"}};
            var userId = "userid";

            await Assert.ThrowsAsync<ServiceException>(() => service.SaveAsync(list, userId));
        }
        
        [Fact]
        public async Task SaveAsync_DeleteIfDtoIsDeleted()
        {
            var repoMock = MocksCreator.RepoRecurrence();
            var service = new RecurrenceService(repoMock.Object, Mapper, _specFactoryMock.Object);

            await service.SaveAsync(new[] {new PlannedRecurrenceDto {Uid = "42", IsDeleted = true}}, null);

            repoMock.Verify(x => x.DeleteAsync("42"));
            repoMock.Verify(x => x.AnyAsync(It.IsAny<IPlannedRecurrenceSpecification>()));
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveAsync_CreateNewIfNoSuchUid()
        {
            var repoMock = MocksCreator.RepoRecurrence();
            var service = new RecurrenceService(repoMock.Object, Mapper, _specFactoryMock.Object);

            await service.SaveAsync(new[] {new PlannedRecurrenceDto {Uid = "42", Task = "42"}}, "userid1");

            repoMock.Verify(x => x.UpsertAsync(It.Is<PlannedRecurrenceEntity>(y =>
                y.Uid == "42" &&
                y.Task == "42" &&
                y.UserId == "userid1")));
            repoMock.Verify(x => x.AnyAsync(It.IsAny<IPlannedRecurrenceSpecification>()));
            repoMock.Verify(x => x.GetByIdAsync("42"));
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveAsync_UpdateIfExists()
        {
            var repoMock = new Mock<IPlannedRecurrenceRepository>();
            repoMock.Setup(x => x.GetByIdAsync("42"))
                .Returns(() => Task.FromResult(new PlannedRecurrenceEntity {Uid = "42"}));
            var service = new RecurrenceService(repoMock.Object, Mapper, _specFactoryMock.Object);

            await service.SaveAsync(new[]
            {
                new PlannedRecurrenceDto
                {
                    Uid = "42",
                    Task = "42",
                    StartDate = new DateTime(2010, 10, 10),
                    EndDate = new DateTime(2011, 11, 11),
                    EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Sunday,
                    EveryMonthDay = "1,2,3",
                    EveryNthDay = 100500
                }
            }, null);

            repoMock.Verify(x => x.TryUpdateVersionAsync(It.Is<PlannedRecurrenceEntity>(y =>
                y.Uid == "42" &&
                y.Task == "42" &&
                y.StartDate == new DateTime(2010, 10, 10) &&
                y.EndDate == new DateTime(2011, 11, 11) &&
                y.EveryWeekday == (RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Sunday) &&
                y.EveryMonthDay == "1,2,3" &&
                y.EveryNthDay == 100500)));
            repoMock.Verify(x => x.GetByIdAsync("42"));
            repoMock.Verify(x => x.AnyAsync(It.IsAny<IPlannedRecurrenceSpecification>()));
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveAsync_DoNotUpdateIfNoChanges()
        {
            var repoMock = new Mock<IPlannedRecurrenceRepository>();
            repoMock.Setup(x => x.GetByIdAsync("42"))
                .Returns(() => Task.FromResult(new PlannedRecurrenceEntity
                {
                    Uid = "42",
                    Task = "42",
                    StartDate = new DateTime(2010, 10, 10),
                    EndDate = new DateTime(2011, 11, 11),
                    EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Sunday,
                    EveryMonthDay = "1,2,3",
                    EveryNthDay = 100500
                }));
            var service = new RecurrenceService(repoMock.Object, Mapper, _specFactoryMock.Object);

            await service.SaveAsync(new[]
            {
                new PlannedRecurrenceDto
                {
                    Uid = "42",
                    Task = "42",
                    StartDate = new DateTime(2010, 10, 10),
                    EndDate = new DateTime(2011, 11, 11),
                    EveryWeekday = RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Sunday,
                    EveryMonthDay = "1,2,3",
                    EveryNthDay = 100500
                }
            }, null);
            
            repoMock.Verify(x => x.GetByIdAsync("42"));
            repoMock.Verify(x => x.AnyAsync(It.IsAny<IPlannedRecurrenceSpecification>()));
            repoMock.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task SaveAsync_ReturnUpdatedCount()
        {
            var repoMock = new Mock<IPlannedRecurrenceRepository>();
            repoMock.Setup(x => x.GetByIdAsync("42"))
                .Returns(() => Task.FromResult(new PlannedRecurrenceEntity {Uid = "42"}));
            repoMock.Setup(x => x.GetByIdAsync("43"))
                .Returns(() => Task.FromResult(new PlannedRecurrenceEntity {Uid = "43", Task = "Old"}));
            repoMock.Setup(x => x.TryUpdateVersionAsync(It.IsAny<PlannedRecurrenceEntity>()))
                .Returns(() => Task.FromResult<(bool, PlannedRecurrenceEntity)>((true, null)));
            var service = new RecurrenceService(repoMock.Object, Mapper, _specFactoryMock.Object);

            var result = await service.SaveAsync(new[]
            {
                new PlannedRecurrenceDto {Uid = "42"},
                new PlannedRecurrenceDto {Uid = "43", Task = "New"},
            }, null);

            Assert.Equal(1, result);
        }
    }
}