using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class DrawStep : ITurnStep
{
    public TurnStep Name { get; } = TurnStep.Draw;

    public void OnStepEnter(GameContext context)
    {
        context.Display.LogStepTransition(Name, context.ActivePlayer.Name);
        context.PriorityPlayer = context.ActivePlayer;

        context.ActivePlayer.DrawCard();
        context.Display.LogMessage($"{context.ActivePlayer.Name} has drawn a card");
    }

    public void HandleAction(GameContext context, PlayerAction action)
    {
        context.TransitionTo(new MainStep1());
    }

    public void OnStepExit(GameContext context)
    {

    }
}
