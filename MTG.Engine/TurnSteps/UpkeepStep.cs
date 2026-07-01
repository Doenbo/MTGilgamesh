using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class UpkeepStep : ITurnStep
{
    public TurnStep Name { get; } = TurnStep.Upkeep;

    public void OnStepEnter(GameContext context)
    {
        context.Display.LogStepTransition(Name, context.ActivePlayer.Name);
        context.PriorityPlayer = context.ActivePlayer;
    }

    public void HandleAction(GameContext context, PlayerAction action)
    {
        context.TransitionTo(new DrawStep());
    }

    public void OnStepExit(GameContext context)
    {

    }
}
