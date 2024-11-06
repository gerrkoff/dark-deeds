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

    public static DayCardList NoDateList()
    {
        return new DayCardList("//*[@data-test-id='card-no-date']//ul");
    }

    public static DayCardsSection CurrentSection()
    {
        return new DayCardsSection("//*[@data-test-id='section-current']");
    }

    public static Menu TaskMenu()
    {
        return new Menu();
    }
}
