namespace DarkDeeds.E2eTests.Components;

public class RootContainer() : X(string.Empty)
{
    public RootContainer SavingStatus()
    {
        Query.Append("//*[@data-test-id='status-saving']");
        return this;
    }

    public RootContainer Loader()
    {
        Query.Append("//*[@data-test-id='loader']");
        return this;
    }
}
