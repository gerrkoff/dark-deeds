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

    public static RootContainer GetRoot()
    {
        return new RootContainer();
    }

    public static SignInForm GetSignInForm()
    {
        return new SignInForm();
    }

    public static EditTaskModal GetEditTaskModal()
    {
        return new EditTaskModal();
    }

    public static CardList GetNoDateList()
    {
        return new CardList("//*[@data-test-id='card-no-date']");
    }

    public static TaskMenu GetTaskMenu()
    {
        return new TaskMenu();
    }
}
