using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class DrawStep : ITurnStep
{
    public TurnStep Name { get; init; } = TurnStep.Draw;

    public void OnStepEnter(GameContext context)
    {
        context.Display.LogStepTransition(Name, context.ActivePlayer.Name);
        context.PriorityPlayer = context.ActivePlayer;

        context.ActivePlayer.DrawCard();
        context.Display.LogMessage($"{context.ActivePlayer.Name} has drawn a card");

        context.AdvanceToNextStep();
    }

    public void HandleAction(GameContext context, PlayerAction action) { }

    public void OnStepExit(GameContext context) { }
}
