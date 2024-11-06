using DarkDeeds.E2eTests.Base;
using DarkDeeds.E2eTests.Common;
using DarkDeeds.E2eTests.Components;
using Xunit;

namespace DarkDeeds.E2eTests;

public class MultiUserTests : BaseTest
{
    [Fact]
    public Task ForeignTasksTest()
    {
        var taskText = RandomizeText("task");
        return Test(async driver =>
        {
            await CreateUserAndLogin(driver);

            driver.CreateTaskViaAddButton(taskText);
            driver.WaitUntilSavingFinished();

            driver.SignOut();

            await CreateUserAndLogin(driver);
            driver.WaitUntilDisappeared(X.OverviewPage().NoDateSection().TaskByText(taskText));
        });
    }

    [Fact]
    public Task ForeignPushNotificationsTest()
    {
        var taskText = RandomizeText("task");
        return Test(async driver =>
        {
            await CreateUserAndLogin(driver);

            driver.OpenNewTab(Url);

            driver.SignOut();

            await CreateUserAndLogin(driver);

            driver.CreateTaskViaAddButton(taskText);
            driver.WaitUntilSavingFinished();

            driver.SwitchToTab(0);

            driver.WaitUntilDisappeared(X.OverviewPage().NoDateSection().TaskByText(taskText));
        });
    }
}
