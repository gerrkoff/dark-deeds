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
            
            Assert.True(result != null);
        }
    }
}