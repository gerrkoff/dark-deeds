using System;
using DarkDeeds.Enums;
using DarkDeeds.Models;
using DarkDeeds.Services.Implementation;
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
            var service = new TaskParserService();

            var result = service.ParseTask("Test!");
            
            Assert.Equal("Test!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Null(result.DateTime);
        }
        
        // #2
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndNoTime()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("1231 Test!");

            var currentYear = DateTime.UtcNow.Year;
            Assert.Equal("Test!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(currentYear,12, 31, 0, 0, 0),  result.DateTime);
        }
        
        // #3
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndNoTime_NotWorkingWithoutSpace()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("0101Test!!!");
            
            Assert.Equal("0101Test!!!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Null(result.DateTime);
        }
        
        // #4
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndTime()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("1231 2359 Test!");
            
            var currentYear = DateTime.UtcNow.Year;
            Assert.Equal("Test!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.ConcreteTime, result.TimeType);
            Assert.Equal(new DateTime(currentYear,12, 31, 23, 59, 0),  result.DateTime);
        }
        
        // #5
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndTime_NotWorkingWithoutSpace()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("0101 0101Test!!!");
            
            var currentYear = DateTime.UtcNow.Year;
            Assert.Equal("0101Test!!!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(currentYear,1, 1, 0, 0, 0),  result.DateTime);
        }
        
        // #6
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndNoTimeWithYear()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("20170101 Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(2017,1, 1, 0, 0, 0),  result.DateTime);
        }
        
        // #7
        [Fact]
        public void ParseTask_ReturnProbableTask()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("Test! ?");
            
            Assert.Equal("Test!", result.Title);
            Assert.True(result.IsProbable);
        }
        
        // #8
        [Fact]
        public void ParseTask_ReturnAllDayLongTaskWithShortDate()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("0220! Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.AllDayLong, result.TimeType);
            Assert.Equal(new DateTime(2019,2, 20, 0, 0, 0),  result.DateTime);
        }
        
        // #9
        [Fact]
        public void ParseTask_ReturnAllDayLongTaskWithLongDate()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("20150220! Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.AllDayLong, result.TimeType);
            Assert.Equal(new DateTime(2015,2, 20, 0, 0, 0),  result.DateTime);
        }
        
        // #10
        [Fact]
        public void ParseTask_IgnoreTimeIfAllDayLong()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("20150606! 2359 Test");
            
            Assert.Equal("2359 Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.AllDayLong, result.TimeType);
            Assert.Equal(new DateTime(2015,6, 6, 0, 0, 0),  result.DateTime);
        }
//        
//        // #11
//        [Fact]
//        public void ParseTask_ReturnTodayTaskThroughExclamationMark()
//        {
//            var service = new TaskParserService();
//
//            var result = service.ParseTask("! Test");
//            
//            Assert.Equal("Test", result.Title);
//            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
//            Assert.Equal(new DateTime(2019,1, 1, 0, 0, 0),  result.DateTime);
//        }
//        
//        // #12
//        [Fact]
//        public void ParseTask_ReturnTomorrowTaskThroughExclamationMark()
//        {
//            var service = new TaskParserService();
//
//            var result = service.ParseTask("!! Test");
//            
//            Assert.Equal("Test", result.Title);
//            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
//            Assert.Equal(new DateTime(2019,1, 2, 0, 0, 0),  result.DateTime);
//        }
//        
//        // #13
//        [Fact]
//        public void ParseTask_ReturnDayAfterAfterTomorrowTaskThroughExclamationMark()
//        {
//            var service = new TaskParserService();
//
//            var result = service.ParseTask("!!!! Test");
//            
//            Assert.Equal("Test", result.Title);
//            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
//            Assert.Equal(new DateTime(2019,1, 4, 0, 0, 0),  result.DateTime);
//        }
//        
//        // #14
//        [Fact]
//        public void ParseTask_ReturnDayAfterTomorrowNextMonthTaskThroughExclamationMark()
//        {
//            // now: 2019, 1, 31
//            var service = new TaskParserService();
//
//            var result = service.ParseTask("!!! Test");
//            
//            Assert.Equal("Test", result.Title);
//            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
//            Assert.Equal(new DateTime(2019,2, 2, 0, 0, 0),  result.DateTime);
//        }
//        
//        // #15
//        [Fact]
//        public void ParseTask_ReturnNextMondayTaskThroughExclamationMark()
//        {
//            // now: 2019, 7, 28
//            var service = new TaskParserService();
//
//            var result = service.ParseTask("!1 Test");
//            
//            Assert.Equal("Test", result.Title);
//            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
//            Assert.Equal(new DateTime(2019,7, 29, 0, 0, 0),  result.DateTime);
//        }
//        
//        // #16
//        [Fact]
//        public void ParseTask_ReturnNextWednesdayTaskThroughExclamationMark()
//        {
//            // now: 2019, 7, 28
//            var service = new TaskParserService();
//
//            var result = service.ParseTask("!3 Test");
//            
//            Assert.Equal("Test", result.Title);
//            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
//            Assert.Equal(new DateTime(2019,7, 31, 0, 0, 0),  result.DateTime);
//        }
//        
//        // #17
//        [Fact]
//        public void ParseTask_ReturnNextFridayTaskThroughExclamationMark()
//        {
//            // now: 2019, 7, 28
//            var service = new TaskParserService();
//
//            var result = service.ParseTask("!5 Test");
//            
//            Assert.Equal("Test", result.Title);
//            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
//            Assert.Equal(new DateTime(2019,8, 2, 0, 0, 0),  result.DateTime);
//        }
        
        #endregion

        #region Parse tasks - unique BE tests
        
        [Fact]
        public void ParseTask_ReturnTask()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("");
            
            Assert.NotNull(result);
        }
        
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndNoTime_ConsiderTimeAdjustment()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("1231 Test!", -159);

            var currentYear = DateTime.UtcNow.Year;
            Assert.Equal("Test!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(currentYear,12, 30, 21, 21, 0),  result.DateTime);
        }
        
        #endregion

        #region Other TaskParserService tests
        
        [Fact]
        public void PrintTasks_ReturnTitle()
        {
            var service = new TaskParserService();

            var result = service.PrintTasks(new[] {new TaskDto
            {
                Title = "Task text"
            }}, 0);

            Assert.Equal("Task text", result);
        }
        
        [Fact]
        public void PrintTasks_ReturnTime()
        {
            var service = new TaskParserService();

            var result = service.PrintTasks(new[] {new TaskDto
            {
                Title = "Task",
                DateTime = new DateTime(2000, 10, 10, 17, 40, 0),
                TimeType = TaskTimeTypeEnum.ConcreteTime
            }});

            Assert.Equal("17:40 Task", result);
        }
        
        [Fact]
        public void PrintTasks_ReturnTimeWithAdjustment()
        {
            var service = new TaskParserService();

            var result = service.PrintTasks(new[] {new TaskDto
            {
                Title = "Task",
                DateTime = new DateTime(2000, 10, 10, 17, 40, 0),
                TimeType = TaskTimeTypeEnum.ConcreteTime
            }}, -80);

            Assert.Equal("16:20 Task", result);
        }
        
        #endregion
    }
}