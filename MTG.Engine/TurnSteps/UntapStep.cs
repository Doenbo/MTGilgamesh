using MTG.Core.Enums;
using MTG.Engine.Events;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class UntapStep : ITurnStep
{
    public TurnStep Name { get; init; } = TurnStep.Untap;

    public void OnStepEnter(GameContext context)
    {
        context.Display.LogGameEvent(new StepTransitionEvent(Name, context.ActivePlayer.Name));

        int untappedCount = 0;
        foreach (var card in context.GetBoardOf(context.ActivePlayer).ToList())
        {
            if (card.IsTapped)
            {
                card.IsTapped = false;
                untappedCount++;
            }
        }

        context.Display.LogInfo($"{context.ActivePlayer.Name} has untapped {untappedCount} Cards");

        context.AdvanceToNextStep();
    }

    public void HandleAction(GameContext context, PlayerAction action) { }

    public void OnStepExit(GameContext context) { }
}
