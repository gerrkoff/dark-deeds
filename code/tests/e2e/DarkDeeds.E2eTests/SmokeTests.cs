﻿using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.E2eTests.Common;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace DarkDeeds.E2eTests
{
    // TODO: create user for each test
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
                var url = $"{Url}/web/api/build-info";
                var result = await httpClient.GetStringAsync(url);
                var version = (string) JObject.Parse(result)["appVersion"];
                _output.WriteLine($"App Version: {version}");
            }
        }

        [Fact]
        public Task SignInTest()
        {
            return Test(_ => { });
        }
        
        [Fact]
        public Task CreateNoDateTest()
        {
            string taskText = RandomizeText("some long & strange name for task");
            return Test(driver =>
            {
                driver.CreateTaskViaAddButton(taskText);
                driver.WaitUntilSavingFinished();
                
                var task = driver.GetTaskByTextInNoDateSection(taskText);
                driver.DeleteTask(task);
                driver.WaitUntilSavingFinished();
            });
        }

        [Fact]
        public Task DragAndDropTaskTest()
        {
            string task1Text = RandomizeText("dnd task 1");
            string task2Text = RandomizeText("dnd task 2");
            string task3Text = RandomizeText("dnd task 3");
            return Test(driver =>
            {
                var overviewSectionParser = new OverviewSectionParser(driver.GetCurrentSection());
                
                var header1 = overviewSectionParser.FindBlock(1).FindDay(2).FindHeader().GetElement();
                driver.CreateTaskViaDayHeader(header1, task1Text);
                
                var header2 = overviewSectionParser.FindBlock(1).FindDay(4).FindHeader().GetElement();
                driver.CreateTaskViaDayHeader(header2, task2Text);
                driver.CreateTaskViaDayHeader(header2, task3Text);
                
                driver.WaitUntilSavingFinished();

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
                
                driver.WaitUntilSavingFinished();
                
                driver.Navigate().Refresh();
                driver.WaitUntilUserLoaded();
                
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
                
                driver.WaitUntilSavingFinished();
            });
        }
        
        [Fact]
        public Task SyncTasksBetweenTabsTest()
        {
            string taskText = RandomizeText("task to sync");
            return Test(driver =>
            {   
                driver.ExecuteJavaScript("window.open()");
                var tabs = driver.WindowHandles;
                driver.SwitchTo().Window(tabs[1]);
                
                driver.Navigate().GoToUrl(Url);
                driver.WaitUntilUserLoaded();
                
                driver.SwitchTo().Window(tabs[0]);
                driver.CreateTaskViaAddButton(taskText);
                driver.WaitUntilSavingFinished();
                driver.GetTaskByTextInNoDateSection(taskText);
                
                driver.SwitchTo().Window(tabs[1]);
                var task = driver.GetTaskByTextInNoDateSection(taskText);
                driver.DeleteTask(task);
                driver.WaitUntilSavingFinished();
                
                driver.SwitchTo().Window(tabs[0]);
                driver.WaitUntilDisappeared(Xpath.TaskWithText(taskText));
            });
        }

        [Fact]
        public Task TimezoneTest()
        {
            DateTime now = DateTime.Now;
            string originalTaskText = RandomizeText("timezone task");
            string taskTextWithDate = $"{now.Month:D2}{now.Day:D2} " + originalTaskText;
            return Test(driver =>
            {
                int expiredDaysCount = ((int) now.DayOfWeek + 6) % 7;
                var overviewSectionParser = new OverviewSectionParser(driver.GetCurrentSection());
                int currentExpiredDaysCount = overviewSectionParser.CountExpiredDays();
                Assert.Equal(expiredDaysCount, currentExpiredDaysCount);
                
                driver.CreateTaskViaAddButton(taskTextWithDate);
                var task = overviewSectionParser.FindBlock(1).FindDay(expiredDaysCount + 1).FindTask(originalTaskText).GetElement();
                driver.DeleteTask(task);
            });
        }
        
        [Fact]
        public Task ScreenSizesTest()
        {
            return Test(driver =>
            {
                var overviewSectionParser = new OverviewSectionParser(driver.GetCurrentSection());
                var days = Enumerable.Range(1, 7)
                    .Select(x => overviewSectionParser.FindBlock(1).FindDay(x).GetElement())
                    .ToList();
                
                driver.Manage().Window.Size = new Size(1400, 1080);

                foreach (var day in days)
                {
                    if (ReferenceEquals(day, days[0]))
                        continue;
                    Assert.Equal(days[0].Location.Y, day.Location.Y);
                    Assert.NotEqual(days[0].Location.X, day.Location.X);
                }
                
                driver.Manage().Window.Size = new Size(523, 700);
                
                foreach (var day in days)
                {
                    if (ReferenceEquals(day, days[0]))
                        continue;
                    Assert.Equal(days[0].Location.X, day.Location.X);
                    Assert.NotEqual(days[0].Location.Y, day.Location.Y);
                }
            });
        }

        [Fact]
        public Task RecurrenceTest()
        {
            return Test(driver =>
            {   
                driver.NavigateToRecurrences();
                driver.WaitUntilRecurrencesLoaded();

                var task = RandomizeText("recurrence");
                var recurrenceTask = $"2359 {task}";
                driver.CreateRecurrence(recurrenceTask);
                driver.CreateTaskRecurrences(2);
                driver.DeleteRecurrence(recurrenceTask);
                
                driver.NavigateToOverview();

                var overviewSectionParser = new OverviewSectionParser(driver.GetCurrentSection());
                var task1 = overviewSectionParser.FindBlock(1).FindDay(7).FindTask($"23:59 {task}").GetElement();
                var task2 = overviewSectionParser.FindBlock(2).FindDay(7).FindTask($"23:59 {task}").GetElement();
                
                driver.DeleteTask(task1);
                driver.DeleteTask(task2);
                
                driver.WaitUntilSavingFinished();
            });
        }
    }
}