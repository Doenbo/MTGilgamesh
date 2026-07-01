using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class CleanUpStep : ITurnStep
{
    public TurnStep Name { get; } = TurnStep.CleanupStep;

    public void OnStepEnter(GameContext context)
    {
        context.Display.LogStepTransition(Name, context.ActivePlayer.Name);
        context.PriorityPlayer = context.ActivePlayer;
    }

    public void HandleAction(GameContext context, PlayerAction action)
    {
        if (action.Type == ActionType.GoToNextPhase) //TODO do we need this if??
        {
            context.AdvanceToNextPlayersTurn();
        }
    }

    public void OnStepExit(GameContext context)
    {

    }
}
