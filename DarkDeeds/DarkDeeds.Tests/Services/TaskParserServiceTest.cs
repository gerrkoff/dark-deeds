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
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Null(result.Date);
            Assert.Null(result.Time);
        }
        
        // #2
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndNoTime()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("1231 Test!");

            Assert.Equal("Test!", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Equal(new DateTime(2019, 12, 31, 0, 0, 0),  result.Date);
            Assert.Null(result.Time);
        }
        
        // #3
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndNoTime_NotWorkingWithoutSpace()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("0101Test!!!");
            
            Assert.Equal("0101Test!!!", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Null(result.Date);
            Assert.Null(result.Time);
        }
        
        // #4
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndTime()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("1231 2359 Test!");
            
            Assert.Equal("Test!", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Equal(new DateTime(2019, 12, 31, 0, 0, 0),  result.Date);
            Assert.Equal(1439, result.Time);
        }
        
        // #5
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndTime_NotWorkingWithoutSpace()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("0101 0101Test!!!");
            
            Assert.Equal("0101Test!!!", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Equal(new DateTime(2019, 1, 1, 0, 0, 0),  result.Date);
            Assert.Null(result.Time);
        }
        
        // #6
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndNoTimeWithYear()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("20170101 Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Equal(new DateTime(2017, 1, 1, 0, 0, 0),  result.Date);
            Assert.Null(result.Time);
        }
        
        // #7
        [Fact]
        public void ParseTask_ReturnProbableTask()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("Test! ?");
            
            Assert.Equal("Test!", result.Title);
            Assert.True(result.IsProbable);
            Assert.Null(result.Date);
            Assert.Null(result.Time);
        }
        
        // #8
        [Fact]
        public void ParseTask_ReturnAdditionalTaskWithDate()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("0220 Test !");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTypeEnum.Additional, result.Type);
            Assert.Equal(new DateTime(2019, 2, 20, 0, 0, 0),  result.Date);
            Assert.Null(result.Time);
        }
        
        // #9
        [Fact]
        public void ParseTask_ReturnAdditionalTaskWithDateAndTime()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("20150220 2359 Test !");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTypeEnum.Additional, result.Type);
            Assert.Equal(new DateTime(2015, 2, 20, 0, 0, 0),  result.Date);
            Assert.Equal(1439, result.Time);
        }
        
        // #10
        [Fact]
        public void ParseTask_AdditionalAndProbable()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("Test !?");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTypeEnum.Additional, result.Type);
            Assert.True(result.IsProbable);
            Assert.Null(result.Date);
            Assert.Null(result.Time);
        }
        
        // #10.1
        [Fact]
        public void ParseTask_ProbableAndAdditional()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("Test ?!");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTypeEnum.Additional, result.Type);
            Assert.True(result.IsProbable);
            Assert.Null(result.Date);
            Assert.Null(result.Time);
        }
        
        // #11
        [Fact]
        public void ParseTask_ReturnTodayTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("! Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Equal(new DateTime(2019, 1, 1, 0, 0, 0),  result.Date);
            Assert.Null(result.Time);
        }
        
        // #12
        [Fact]
        public void ParseTask_ReturnTomorrowTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("!! Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Equal(new DateTime(2019, 1, 2, 0, 0, 0),  result.Date);
            Assert.Null(result.Time);
        }
        
        // #13
        [Fact]
        public void ParseTask_ReturnDayAfterAfterTomorrowTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("!!!! Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Equal(new DateTime(2019, 1, 4, 0, 0, 0),  result.Date);
            Assert.Null(result.Time);
        }
        
        // #14
        [Fact]
        public void ParseTask_ReturnDayAfterTomorrowNextMonthTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock(2019, 1, 31));

            var result = service.ParseTask("!!! Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Equal(new DateTime(2019, 2, 2, 0, 0, 0),  result.Date);
            Assert.Null(result.Time);
        }
        
        // #15
        [Fact]
        public void ParseTask_ReturnNextMondayTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock(2019, 7,28));

            var result = service.ParseTask("!1 Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Equal(new DateTime(2019, 7, 29, 0, 0, 0),  result.Date);
            Assert.Null(result.Time);
        }
        
        // #16
        [Fact]
        public void ParseTask_ReturnNextWednesdayTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock(2019, 7, 28));

            var result = service.ParseTask("!3 Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Equal(new DateTime(2019, 7, 31, 0, 0, 0),  result.Date);
            Assert.Null(result.Time);
        }
        
        // #17
        [Fact]
        public void ParseTask_ReturnNextFridayTaskThroughExclamationMark()
        {
            var service = new TaskParserService(dateServiceMock(2019, 7, 28));

            var result = service.ParseTask("!5 Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Equal(new DateTime(2019, 8, 2, 0, 0, 0),  result.Date);
            Assert.Null(result.Time);
        }
        
        // #18
        [Fact]
        public void ParseTask_ExclamationMark11IsNotWeekShiftPattern()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("!11 Test");
            
            Assert.Equal("!11 Test", result.Title);
        }
        
        // #19
        [Fact]
        public void ParseTask_DateWithExclamation()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("1231! Test");
            
            Assert.Equal("1231! Test", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Null(result.Date);
            Assert.Null(result.Time);
        }
        
        #endregion
        
        #region Other TaskParserService tests
        
        [Fact]
        public void ParseTask_IgnoreDateTaskWithTime()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.ParseTask("1010 Test!", ignoreDate: true);
            
            Assert.Equal("Test!", result.Title);
            Assert.Equal(TaskTypeEnum.Simple, result.Type);
            Assert.Null(result.Date);
            Assert.Equal(610, result.Time);
        }
        
        [Fact]
        public void PrintTasks_ReturnTitle()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.PrintTasks(new[] {new TaskDto
            {
                Title = "Task text"
            }});

            Assert.Equal("Task text", result);
        }
        
        [Fact]
        public void PrintTasks_ReturnTime()
        {
            var service = new TaskParserService(dateServiceMock());

            var result = service.PrintTasks(new[] {new TaskDto
            {
                Title = "Task",
                Date = new DateTime(2000, 10, 10, 0, 0, 0),
                Time = 1060
            }});

            Assert.Equal("17:40 Task", result);
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