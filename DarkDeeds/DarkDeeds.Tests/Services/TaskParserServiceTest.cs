using System;
using DarkDeeds.Enums;
using DarkDeeds.Models;
using DarkDeeds.Services.Implementation;
using DarkDeeds.Services.Interface;
using Moq;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public class TaskParserServiceTest : BaseTest
    {
        #region Parse tasks - mirrored FE tests
        
        // These tests should be synced with FE TaskConverter.convertStringToModel tests 
        
        // #1
        [Fact]
        public void ParseTask_ReturnTaskWithNoDateAndNoTime()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("Test!");
            
            Assert.Equal("Test!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Null(result.Date);
        }
        
        // #2
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndNoTime()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("1231 Test!");

            Assert.Equal("Test!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(2019, 12, 31, 0, 0, 0),  result.Date);
        }
        
        // #3
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndNoTime_NotWorkingWithoutSpace()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("0101Test!!!");
            
            Assert.Equal("0101Test!!!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Null(result.Date);
        }
        
        // #4
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndTime()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("1231 2359 Test!");
            
            Assert.Equal("Test!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.ConcreteTime, result.TimeType);
            Assert.Equal(new DateTime(2019, 12, 31, 23, 59, 0),  result.Date);
        }
        
        // #5
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndTime_NotWorkingWithoutSpace()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("0101 0101Test!!!");
            
            Assert.Equal("0101Test!!!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(2019, 1, 1, 0, 0, 0),  result.Date);
        }
        
        // #6
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndNoTimeWithYear()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("20170101 Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(2017, 1, 1, 0, 0, 0),  result.Date);
        }
        
        // #7
        [Fact]
        public void ParseTask_ReturnProbableTask()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("Test! ?");
            
            Assert.Equal("Test!", result.Title);
            Assert.True(result.IsProbable);
        }
        
        // #8
        [Fact]
        public void ParseTask_ReturnAllDayLongTaskWithShortDate()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("0220! Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.AllDayLong, result.TimeType);
            Assert.Equal(new DateTime(2019, 2, 20, 0, 0, 0),  result.Date);
        }
        
        // #9
        [Fact]
        public void ParseTask_ReturnAllDayLongTaskWithLongDate()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("20150220! Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.AllDayLong, result.TimeType);
            Assert.Equal(new DateTime(2015, 2, 20, 0, 0, 0),  result.Date);
        }
        
        // #10
        [Fact]
        public void ParseTask_IgnoreTimeIfAllDayLong()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("20150606! 2359 Test");
            
            Assert.Equal("2359 Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.AllDayLong, result.TimeType);
            Assert.Equal(new DateTime(2015, 6, 6, 0, 0, 0),  result.Date);
        }
        
        // #11
        [Fact]
        public void ParseTask_ReturnTodayTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("! Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(2019, 1, 1, 0, 0, 0),  result.Date);
        }
        
        // #12
        [Fact]
        public void ParseTask_ReturnTomorrowTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("!! Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(2019, 1, 2, 0, 0, 0),  result.Date);
        }
        
        // #13
        [Fact]
        public void ParseTask_ReturnDayAfterAfterTomorrowTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("!!!! Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(2019, 1, 4, 0, 0, 0),  result.Date);
        }
        
        // #14
        [Fact]
        public void ParseTask_ReturnDayAfterTomorrowNextMonthTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock(2019, 1, 31));

            var result = service.ParseTask("!!! Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(2019, 2, 2, 0, 0, 0),  result.Date);
        }
        
        // #15
        [Fact]
        public void ParseTask_ReturnNextMondayTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock(2019, 7,28));

            var result = service.ParseTask("!1 Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(2019, 7, 29, 0, 0, 0),  result.Date);
        }
        
        // #16
        [Fact]
        public void ParseTask_ReturnNextWednesdayTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock(2019, 7, 28));

            var result = service.ParseTask("!3 Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(2019, 7, 31, 0, 0, 0),  result.Date);
        }
        
        // #17
        [Fact]
        public void ParseTask_ReturnNextFridayTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock(2019, 7, 28));

            var result = service.ParseTask("!5 Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(2019, 8, 2, 0, 0, 0),  result.Date);
        }
        
        // #18
        [Fact]
        public void ParseTask_ExclamationMark11IsNotWeekShiftPattern()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("!11 Test");
            
            Assert.Equal("!11 Test", result.Title);
        }
        
        #endregion

        #region Parse tasks - unique BE tests
        
        [Fact]
        public void ParseTask_ReturnTask()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("");
            
            Assert.NotNull(result);
        }
        
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndNoTime_ConsiderTimeAdjustment()
        {
            var service = new TaskParserService(dateServiceMock(2010));

            var result = service.ParseTask("1231 Test!", -159);

            Assert.Equal("Test!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(2010,12, 30, 21, 21, 0),  result.Date);
        }
        
        #endregion

        #region Other TaskParserService tests
        
        [Fact]
        public void PrintTasks_ReturnTitle()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.PrintTasks(new[] {new TaskDto
            {
                Title = "Task text"
            }}, 0);

            Assert.Equal("Task text", result);
        }
        
        [Fact]
        public void PrintTasks_ReturnTime()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.PrintTasks(new[] {new TaskDto
            {
                Title = "Task",
                Date = new DateTime(2000, 10, 10, 17, 40, 0),
                TimeType = TaskTimeTypeEnum.ConcreteTime
            }});

            Assert.Equal("17:40 Task", result);
        }
        
        [Fact]
        public void PrintTasks_ReturnTimeWithAdjustment()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.PrintTasks(new[] {new TaskDto
            {
                Title = "Task",
                Date = new DateTime(2000, 10, 10, 17, 40, 0),
                TimeType = TaskTimeTypeEnum.ConcreteTime
            }}, -80);

            Assert.Equal("16:20 Task", result);
        }
        
        #endregion
        
        #region Helpers

        private IDateService dateServiceMock(int year = 2019, int month = 1, int date = 1)
        {
            var mock = new Mock<IDateService>();
            mock.SetupGet(x => x.Today).Returns(new DateTime(year, month, date));
            return mock.Object;
        }
        
        #endregion
    }
}