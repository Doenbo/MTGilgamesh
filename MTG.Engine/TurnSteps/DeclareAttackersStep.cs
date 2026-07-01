using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class DeclareAttackersStep : ITurnStep
{
    public TurnStep Name { get; } = TurnStep.DeclareAttackers;

    public void OnStepEnter(GameContext context)
    {
        context.Display.LogStepTransition(Name, context.ActivePlayer.Name);
        context.PriorityPlayer = context.ActivePlayer;
    }

    public void HandleAction(GameContext context, PlayerAction action)
    { 
        context.TransitionTo(new DeclareBlockersStep());
    }

    public void OnStepExit(GameContext context)
    {

    }
}