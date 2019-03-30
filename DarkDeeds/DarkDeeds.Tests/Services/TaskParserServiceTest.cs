using System;
using DarkDeeds.Enums;
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
        public void ParseTask_ReturnTaskWithDateAndAfterTimeWithYear()
        {
            var service = new TaskParserService();

            var result = service.ParseTask("20171010 >0211 Test");
            
            Assert.Equal("Test", result.Title);
            Assert.Equal(TaskTimeTypeEnum.AfterTime, result.TimeType);
            Assert.Equal(new DateTime(2017,10, 10, 2, 11, 0),  result.DateTime);
        }
    }
}