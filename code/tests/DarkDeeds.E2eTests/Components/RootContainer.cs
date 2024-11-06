using DarkDeeds.E2eTests.Common;

namespace DarkDeeds.E2eTests.Components;

public class RootContainer() : X("//*[@id='root']")
{
    public RootContainer GetAddTaskButton()
    {
        Query.Append("//*[@data-test-id='btn-add-task']");
        return this;
    }

    public RootContainer GetSavingStatus()
    {
        Query.Append("//*[@data-test-id='status-saving']");
        return this;
    }

    public RootContainer GetNavbar()
    {
        Query.Append($"//*{XpathHelper.ClassContains("navbar")}");
        return this;
    }
}
