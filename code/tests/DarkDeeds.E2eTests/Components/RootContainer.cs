namespace DarkDeeds.E2eTests.Components;

public class RootContainer() : X("//*[@id='root']")
{
    public RootContainer AddTaskButton()
    {
        Query.Append("//*[@data-test-id='btn-add-task']");
        return this;
    }

    public RootContainer SavingStatus()
    {
        Query.Append("//*[@data-test-id='status-saving']");
        return this;
    }

    public RootContainer SignOutButton()
    {
        Query.Append("//*[@data-test-id='btn-signout']");
        return this;
    }
}
