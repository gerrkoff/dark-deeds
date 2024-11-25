namespace DarkDeeds.E2eTests.Components;

public class Button(string path) : X(path)
{
    public Button Loader()
    {
        Query.Append("//*[@data-test-id='btn-loader']");
        return this;
    }
}
