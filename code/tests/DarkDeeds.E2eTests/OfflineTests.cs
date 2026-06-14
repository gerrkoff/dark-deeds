using DarkDeeds.E2eTests.Attributes;
using DarkDeeds.E2eTests.Backend;
using DarkDeeds.E2eTests.Base;
using DarkDeeds.E2eTests.Common;
using DarkDeeds.E2eTests.Components;
using Xunit;

namespace DarkDeeds.E2eTests;

public class OfflineTests : BaseTest
{
    [ProductionBuildFact]
    public Task OfflineEditSurvivesReloadAndSyncsTest()
    {
        var onlineTask = RandomizeText("online task");
        var offlineTask = RandomizeText("offline task");
        return Test(async driver =>
        {
            var user = await CreateUserAndLogin(driver);

            // Create the baseline task online.
            driver.CreateTaskViaAddButton(onlineTask);
            driver.WaitUntilSavingFinished();

            // Service worker takes control after a reload.
            driver.Navigate().Refresh();
            driver.WaitUntilUserLoaded();
            driver.WaitUntilServiceWorkerControls();

            // Edit offline.
            driver.GoOffline();
            driver.CreateTaskViaAddButton(offlineTask);
            driver.GetElement(X.OverviewPage().NoDateSection().TaskByText(offlineTask));

            // Offline reload keeps both tasks.
            driver.Navigate().Refresh();
            driver.WaitUntilUserLoaded();
            driver.GetElement(X.OverviewPage().NoDateSection().TaskByText(onlineTask));
            driver.GetElement(X.OverviewPage().NoDateSection().TaskByText(offlineTask));

            // Back online: the offline edit syncs to the backend.
            driver.GoOnline();
            await BackendVerifier.WaitUntilHasTaskAsync(user, offlineTask);

            // Both tasks survive an online reload.
            driver.Navigate().Refresh();
            driver.WaitUntilUserLoaded();
            driver.GetElement(X.OverviewPage().NoDateSection().TaskByText(onlineTask));
            driver.GetElement(X.OverviewPage().NoDateSection().TaskByText(offlineTask));

            // The backend has both tasks.
            var titles = await BackendApi.GetTaskTitlesAsync(user);
            Assert.Contains(onlineTask, titles);
            Assert.Contains(offlineTask, titles);
        });
    }
}
