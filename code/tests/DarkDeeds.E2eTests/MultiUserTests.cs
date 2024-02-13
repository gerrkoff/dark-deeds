using DarkDeeds.E2eTests.Base;
using DarkDeeds.E2eTests.Common;
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

            driver.NavigateToSettings();
            driver.GetSignOutButton().Click();

            await CreateUserAndLogin(driver);
            driver.WaitUntilTaskDisappeared(taskText);
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
            driver.NavigateToSettings();
            driver.GetSignOutButton().Click();
            await CreateUserAndLogin(driver);

            driver.CreateTaskViaAddButton(taskText);
            driver.WaitUntilSavingFinished();

            driver.SwitchToTab(0);

            driver.WaitUntilTaskDisappeared(taskText);
        });
    }
}
