namespace DarkDeeds.E2eTests.Components;

public class SignInForm() : X("//*[@data-test-id='form-signin']")
{
    public SignInForm GetUsernameInput()
    {
        Query.Append("//input[@id='username']");
        return this;
    }

    public SignInForm GetPasswordInput()
    {
        Query.Append("//input[@id='password']");
        return this;
    }

    public SignInForm GetSubmitButton()
    {
        Query.Append("//button[@type='submit']");
        return this;
    }
}
