using System.Drawing;
using DarkDeeds.E2eTests.Base;
using DarkDeeds.E2eTests.Common;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Interactions;
using Xunit;
using Xunit.Abstractions;

namespace DarkDeeds.E2eTests;

public class SmokeTests(ITestOutputHelper output) : UserLoginTest
{
    [Fact]
    public async void GetBuildVersionTest()
    {
        using var httpClient = CreateHttpClient();
        var url = new Uri("api/be/build-info", UriKind.Relative);
        var result = await httpClient.GetStringAsync(url);
        var version = (string)JObject.Parse(result)["appVersion"];
        output.WriteLine($"App Version: {version}");
    }

    [Fact]
    public Task SignInTest()
    {
        return Test(_ => Task.CompletedTask);
    }

    [Fact]
    public Task CreateNoDateTest()
    {
        var taskText = RandomizeText("some long & strange name for task");
        return Test(driver =>
        {
            driver.CreateTaskViaAddButton(taskText);
            driver.WaitUntilSavingFinished();

            var task = driver.GetTaskByTextInNoDateSection(taskText);
            driver.DeleteTask(task);
            driver.WaitUntilSavingFinished();

            return Task.CompletedTask;
        });
    }

    [Fact]
    public Task DragAndDropTaskTest()
    {
        var task1Text = RandomizeText("dnd task 1");
        var task2Text = RandomizeText("dnd task 2");
        var task3Text = RandomizeText("dnd task 3");
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

            return Task.CompletedTask;
        });
    }

    [Fact]
    public Task SyncTasksBetweenTabsTest()
    {
        var taskText = RandomizeText("task to sync");
        return Test(driver =>
        {
            driver.OpenNewTab(Url);

            driver.SwitchToTab(0);
            driver.CreateTaskViaAddButton(taskText);
            driver.WaitUntilSavingFinished();
            driver.GetTaskByTextInNoDateSection(taskText);

            driver.SwitchToTab(1);
            var task = driver.GetTaskByTextInNoDateSection(taskText);
            driver.DeleteTask(task);
            driver.WaitUntilSavingFinished();

            driver.SwitchToTab(0);
            driver.WaitUntilTaskDisappeared(taskText);

            return Task.CompletedTask;
        });
    }

    [Fact]
    public Task TimezoneTest()
    {
        var now = DateTime.Now;
        var originalTaskText = RandomizeText("timezone task");
        var taskTextWithDate = $"{now.Month:D2}{now.Day:D2} " + originalTaskText;
        return Test(driver =>
        {
            var expiredDaysCount = ((int)now.DayOfWeek + 6) % 7;
            var overviewSectionParser = new OverviewSectionParser(driver.GetCurrentSection());
            var currentExpiredDaysCount = overviewSectionParser.CountExpiredDays();
            Assert.Equal(expiredDaysCount, currentExpiredDaysCount);

            driver.CreateTaskViaAddButton(taskTextWithDate);
            overviewSectionParser.FindBlock(1).FindDay(expiredDaysCount + 1).FindTask(originalTaskText).GetElement();

            return Task.CompletedTask;
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

            return Task.CompletedTask;
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

            driver.NavigateToOverview();

            var overviewSectionParser = new OverviewSectionParser(driver.GetCurrentSection());
            overviewSectionParser.FindBlock(1).FindDay(7).FindTask($"23:59 {task}").GetElement();
            overviewSectionParser.FindBlock(2).FindDay(7).FindTask($"23:59 {task}").GetElement();

            return Task.CompletedTask;
        });
    }
}
