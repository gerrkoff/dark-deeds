using DarkDeeds.E2eTests.Common;

namespace DarkDeeds.E2eTests.Components;

public class Menu() : X($"//div{XpathHelper.ClassContains("list-group")}")
{
    public Button AddButton()
    {
        return new Button($"{this}//button[@data-test-id='btn-add-item']");
    }

    public Button DeleteButton()
    {
        return new Button($"{this}//button[@data-test-id='btn-delete-item']");
    }
}
