using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class UntapStep : ITurnStep
{
    public TurnStep Name { get; init; } = TurnStep.Untap;

    public void OnStepEnter(GameContext context)
    {
        context.Display?.LogStepTransition(Name, context.ActivePlayer.Name);

        int untappedCount = 0;
        foreach (var card in context.GetBoardOf(context.ActivePlayer))
        {
            if (card.IsTapped)
            {
                card.IsTapped = false;
                untappedCount++;
            }
        }

        context.Display?.LogMessage($"{context.ActivePlayer.Name} has untapped {untappedCount} Cards");

        context.AdvanceToNextStep();
    }

    public void HandleAction(GameContext context, PlayerAction action) { }

    public void OnStepExit(GameContext context) { }
}
