namespace DarkDeeds.E2eTests.Components;

public class SettingsPage() : X(string.Empty)
{
    public SettingsPage SignOutButton()
    {
        Query.Append("//*[@data-test-id='btn-signout']");
        return this;
    }
}
