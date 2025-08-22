using System.Drawing;
using DarkDeeds.E2eTests.Base;
using DarkDeeds.E2eTests.Common;
using DarkDeeds.E2eTests.Components;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Interactions;
using Xunit;
using Xunit.Abstractions;

namespace DarkDeeds.E2eTests;

// TODO: add test for weekly tasks
public class SmokeTests(ITestOutputHelper output) : UserLoginTest
{
    [Fact]
    public async Task GetBuildVersionTest()
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
        return Test(_ => { });
    }

    [Fact]
    public Task CreateNoDateTest()
    {
        var taskText = RandomizeText("some long & strange name for task");
        return Test(driver =>
        {
            driver.CreateTaskViaAddButton(taskText);
            driver.WaitUntilSavingFinished();

            var task = driver.GetElement(X.OverviewPage().NoDateSection().TaskByText(taskText));
            driver.DeleteTask(task);
            driver.WaitUntilSavingFinished();
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
            var header1 = driver.GetElement(X.OverviewPage().CurrentSection().Block(1).Day(2).DateHeader());
            driver.CreateTaskViaDayHeader(header1, task1Text);

            var header2 = driver.GetElement(X.OverviewPage().CurrentSection().Block(1).Day(4).DateHeader());
            driver.CreateTaskViaDayHeader(header2, task2Text);
            driver.CreateTaskViaDayHeader(header2, task3Text);

            driver.WaitUntilSavingFinished();

            var task1Original = driver.GetElement(X.OverviewPage().CurrentSection().Block(1).Day(2).List().TaskByText(task1Text));
            var task2Original = driver.GetElement(X.OverviewPage().CurrentSection().Block(1).Day(4).List().TaskByText(task2Text));

            var actions = new Actions(driver);

            actions
                .ClickAndHold(task1Original)
                .MoveToElement(task2Original)
                .MoveByOffset(0, task2Original.Size.Height / 4)
                .Release()
                .Build()
                .Perform();

            driver.WaitUntilSavingFinished();

            driver.Navigate().Refresh();
            driver.WaitUntilUserLoaded();

            var task1Saved = driver.GetElement(X.OverviewPage().CurrentSection().Block(1).Day(4).List().TaskByText(task1Text));
            var task2Saved = driver.GetElement(X.OverviewPage().CurrentSection().Block(1).Day(4).List().TaskByText(task2Text));
            var task3Saved = driver.GetElement(X.OverviewPage().CurrentSection().Block(1).Day(4).List().TaskByText(task3Text));

            Assert.True(task1Saved.Location.X == task2Saved.Location.X);
            Assert.True(task1Saved.Location.X == task3Saved.Location.X);
            Assert.True(task1Saved.Location.Y > task2Saved.Location.Y);
            Assert.True(task1Saved.Location.Y < task3Saved.Location.Y);
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
            driver.GetElement(X.OverviewPage().NoDateSection().TaskByText(taskText));

            driver.SwitchToTab(1);
            var task = driver.GetElement(X.OverviewPage().NoDateSection().TaskByText(taskText));
            driver.DeleteTask(task);
            driver.WaitUntilSavingFinished();

            driver.SwitchToTab(0);
            driver.WaitUntilDisappeared(X.OverviewPage().NoDateSection().TaskByText(taskText));
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
            var currentExpiredDaysCount = driver.CountElements(X.OverviewPage().CurrentSection().Block(1).Expired());
            Assert.Equal(expiredDaysCount, currentExpiredDaysCount);

            driver.CreateTaskViaAddButton(taskTextWithDate);
            driver.GetElement(X.OverviewPage().CurrentSection().Block(1).Day(expiredDaysCount + 1).List().TaskByText(originalTaskText));
        });
    }

    [Fact]
    public Task ScreenSizesTest()
    {
        return Test(driver =>
        {
            var days = Enumerable.Range(1, 7)
                .Select(x => driver.GetElement(X.OverviewPage().CurrentSection().Block(1).Day(x)))
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

            var task = RandomizeText("recurrence");
            var recurrenceTask = $"2359 {task}";
            driver.CreateRecurrence(recurrenceTask);
            driver.CreateTaskRecurrences();
            driver.NavigateToOverview();

            driver.ElementExists(X.OverviewPage().CurrentSection().Block(1).Day(7).List().TaskByText($"23:59 {task}"));
            driver.ElementExists(X.OverviewPage().CurrentSection().Block(2).Day(7).List().TaskByText($"23:59 {task}"));
        });
    }
}
