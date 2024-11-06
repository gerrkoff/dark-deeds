using System.Text;

namespace DarkDeeds.E2eTests.Components;

public abstract class X(string initPath)
{
    protected StringBuilder Query { get; } = new(initPath);

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

    public static CardList NoDateList()
    {
        return new CardList("//*[@data-test-id='card-no-date']");
    }

    public static TaskMenu TaskMenu()
    {
        return new TaskMenu();
    }
}
