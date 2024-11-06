using DarkDeeds.E2eTests.Common;

namespace DarkDeeds.E2eTests.Components;

public class Navbar() : X($"//*{XpathHelper.ClassContains("navbar")}")
{
    public Navbar Overview()
    {
        Query.Append("//*[@data-test-id='nav-overview']");
        return this;
    }

    public Navbar Recurrences()
    {
        Query.Append("//*[@data-test-id='nav-recurrent']");
        return this;
    }

    public Navbar Settings()
    {
        Query.Append("//*[@data-test-id='nav-settings']");
        return this;
    }
}
