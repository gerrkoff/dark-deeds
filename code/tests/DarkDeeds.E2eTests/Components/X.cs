using System.Text;

namespace DarkDeeds.E2eTests.Components;

public abstract class X(string path)
{
    protected StringBuilder Query { get; } = new(path);

    public static implicit operator string(X x)
    {
        return x.Query.ToString();
    }

    public override string ToString()
    {
        return Query.ToString();
    }

    public static RootContainer Root()
    {
        return new RootContainer();
    }

    public static Navbar Navbar()
    {
        return new Navbar();
    }

    public static SignInForm SignInForm()
    {
        return new SignInForm();
    }

    public static EditTaskModal EditTaskModal()
    {
        return new EditTaskModal();
    }

    public static Menu TaskMenu()
    {
        return new Menu();
    }

    public static OverviewPage OverviewPage()
    {
        return new OverviewPage();
    }

    public static SettingsPage SettingsPage()
    {
        return new SettingsPage();
    }
}
