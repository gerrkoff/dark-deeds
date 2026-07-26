namespace DD.TerminalClient.Domain.Application;

// The result of one pure reducer transition: the next state plus any effects the loop must run.
public sealed record ApplicationTransition(ApplicationState State, IReadOnlyList<ApplicationEffect> Effects)
{
    public static ApplicationTransition Of(ApplicationState state)
    {
        return new ApplicationTransition(state, []);
    }

    public static ApplicationTransition Of(ApplicationState state, ApplicationEffect effect)
    {
        return new ApplicationTransition(state, [effect]);
    }
}
