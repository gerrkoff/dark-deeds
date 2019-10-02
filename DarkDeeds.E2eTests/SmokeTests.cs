using System;
using System.Threading;
using DarkDeeds.E2eTests.Common;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace DarkDeeds.E2eTests
{
    public class SmokeTests : BaseTest
    {
        private readonly ITestOutputHelper _output;

        public SmokeTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public async void GetBuildVersionTest()
        {
            using (var httpClient = CreateHttpClient())
            {
                var url = $"{ApiUrl}/api/build-info";
                var result = await httpClient.GetStringAsync(url);
                var version = (string) JObject.Parse(result)["version"];
                _output.WriteLine($"App Version: {version}");
            }
        }

        [Fact]
        public void SignInTest()
        {
            Test(driver =>
            {
                driver.SignIn(Username, Password);
            });
        }
        
        [Fact]
        public void CreateNoDateTest()
        {
            string taskText = RandomizeText("some long & strange name for task");
            Test(driver =>
            {
                driver.SignIn(Username, Password);
                driver.WaitUntillUserLoaded();

                driver.CreateTaskViaAddButton(taskText);
                driver.WaitUntillSavingFinished();
                
                var task = driver.GetTaskByTextInNoDateSection(taskText);
                driver.DeleteTask(task);
                driver.WaitUntillSavingFinished();
            });
        }

        [Fact]
        public void DragAndDropTaskTest()
        {
            string task1Text = RandomizeText("dnd task 1");
            string task2Text = RandomizeText("dnd task 2");
            string task3Text = RandomizeText("dnd task 3");
            Test(driver =>
            {
                driver.SignIn(Username, Password);
                driver.WaitUntillUserLoaded();

                var overviewSectionParser = new OverviewSectionParser(driver.GetCurrentSection());
                
                var header1 = overviewSectionParser.FindBlock(1).FindDay(2).FindHeader().GetElement();
                driver.ScrollToElement(header1);
                driver.CreateTaskViaDayHeader(header1, task1Text);
                
                var header2 = overviewSectionParser.FindBlock(1).FindDay(4).FindHeader().GetElement();
                driver.ScrollToElement(header2);
                driver.CreateTaskViaDayHeader(header2, task2Text);
                driver.CreateTaskViaDayHeader(header2, task3Text);
                
                driver.WaitUntillSavingFinished();

                var task1Original = overviewSectionParser.FindBlock(1).FindDay(2).FindTask(task1Text).GetElement();
                var task2Original = overviewSectionParser.FindBlock(1).FindDay(4).FindTask(task2Text).GetElement();

                var actions = new Actions(driver);

                actions
                    .ClickAndHold(task1Original)
                    .MoveToElement(task2Original)
                    .Build()
                    .Perform();
                actions
                    .MoveByOffset(task2Original.Size.Width / 2, task2Original.Size.Height / 2)
                    .Release()
                    .Build()
                    .Perform();
                
                driver.WaitUntillSavingFinished();
                
                driver.Navigate().Refresh();
                driver.WaitUntillUserLoaded();
                
                overviewSectionParser = new OverviewSectionParser(driver.GetCurrentSection());
                var task1Saved = overviewSectionParser.FindBlock(1).FindDay(4).FindTask(task1Text).GetElement();
                var task2Saved = overviewSectionParser.FindBlock(1).FindDay(4).FindTask(task2Text).GetElement();
                var task3Saved = overviewSectionParser.FindBlock(1).FindDay(4).FindTask(task3Text).GetElement();
                
                Assert.True(task1Saved.Location.X == task2Saved.Location.X);
                Assert.True(task1Saved.Location.X == task3Saved.Location.X);
                Assert.True(task1Saved.Location.Y > task2Saved.Location.Y);
                Assert.True(task1Saved.Location.Y < task3Saved.Location.Y);

                driver.DeleteTask(task1Saved);
                driver.DeleteTask(task2Saved);
                driver.DeleteTask(task3Saved);
                
                driver.WaitUntillSavingFinished();
            });
        }
        
        [Fact]
        public void SyncTasksBetweenTabsTest()
        {
            string taskText = RandomizeText("task to sync");
            Test(driver =>
            {
                driver.SignIn(Username, Password);
                driver.WaitUntillUserLoaded();
                
                driver.ExecuteJavaScript("window.open()");
                var tabs = driver.WindowHandles;
                driver.SwitchTo().Window(tabs[1]);
                
                driver.Navigate().GoToUrl(Url);
                driver.WaitUntillUserLoaded();
                
                driver.SwitchTo().Window(tabs[0]);
                driver.CreateTaskViaAddButton(taskText);
                driver.WaitUntillSavingFinished();
                driver.GetTaskByTextInNoDateSection(taskText);
                
                driver.SwitchTo().Window(tabs[1]);
                var task = driver.GetTaskByTextInNoDateSection(taskText);
                driver.DeleteTask(task);
                driver.WaitUntillSavingFinished();
                
                driver.SwitchTo().Window(tabs[0]);
                driver.WaitUntilDisappeared(Xpath.TaskWithText(taskText));
            });
        }

        [Fact]
        public void TimezoneTest()
        {
            DateTime now = DateTime.Now;
            string originalTaskText = RandomizeText("timezone task");
            string taskTextWithDate = $"{now.Month:D2}{now.Day:D2} " + originalTaskText;
            Test(driver =>
            {
                driver.SignIn(Username, Password);
                int expiredDaysCount = ((int) now.DayOfWeek + 6) % 7;
                var overviewSectionParser = new OverviewSectionParser(driver.GetCurrentSection());
                int currentExpiredDaysCount = overviewSectionParser.CountExpiredDays();
                Assert.Equal(expiredDaysCount, currentExpiredDaysCount);
                
                driver.CreateTaskViaAddButton(taskTextWithDate);
                var task = overviewSectionParser.FindBlock(1).FindDay(expiredDaysCount + 1).FindTask(originalTaskText).GetElement();
                driver.DeleteTask(task);
            });
        }
    }
}