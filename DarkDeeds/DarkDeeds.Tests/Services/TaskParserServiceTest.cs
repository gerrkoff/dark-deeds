using System;
using DarkDeeds.Enums;
using DarkDeeds.Models;
using DarkDeeds.Services.Implementation;
using Xunit;

namespace DarkDeeds.Tests.Services
{
    public class TaskParserServiceTest : BaseTest
    {
        [Fact]
        public void ParseTask_ReturnTask()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("");
            
            Assert.NotNull(result);
        }
        
        [Fact]
        public void ParseTask_ReturnTaskWithNoDateAndNoTime()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("Test!");
            
            Assert.Equal("Test!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Null(result.DateTime);
        }
        
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
        
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndNoTime_NotWorkingWithoutSpace()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("0101Test!!!");
            
            Assert.Equal("0101Test!!!", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Null(result.DateTime);
        }
        
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
        
        [Fact]
        public void ParseTask_ReturnTaskWithDateAndNoTimeWithYear()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("20170101 Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.NoTime, result.TimeType);
            Assert.Equal(new DateTime(2017,1, 1, 0, 0, 0),  result.DateTime);
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
    }
}