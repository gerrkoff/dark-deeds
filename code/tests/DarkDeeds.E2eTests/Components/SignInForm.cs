namespace DarkDeeds.E2eTests.Components;

public class SignInForm() : X("//*[@data-test-id='form-signin']")
{
    public SignInForm UsernameInput()
    {
        Query.Append("//input[@id='username']");
        return this;
    }

    public SignInForm PasswordInput()
    {
        Query.Append("//input[@id='password']");
        return this;
    }

    public SignInForm SubmitButton()
    {
        Query.Append("//button[@type='submit']");
        return this;
    }
}
