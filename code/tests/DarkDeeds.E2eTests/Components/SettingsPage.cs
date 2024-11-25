namespace DarkDeeds.E2eTests.Components;

public class SettingsPage() : X(string.Empty)
{
    public Button SignOutButton()
    {
        return new Button($"{this}//*[@data-test-id='btn-signout']");
    }
}
