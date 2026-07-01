using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class UntapStep : ITurnStep
{
    public TurnStep Name { get; } = TurnStep.Untap;

    public void OnStepEnter(GameContext context)
    {
        context.Display.LogStepTransition(Name, context.ActivePlayer.Name);
        context.PriorityPlayer = context.ActivePlayer;

        var playerPermanents = context.Battlefield.Where(card => card.Controller == context.ActivePlayer);

        int untappedCount = 0;
        foreach (var card in playerPermanents)
        {
            if (card.IsTapped)
            {
                card.IsTapped = false;
                untappedCount++;
            }
        }

        context.Display.LogMessage($"{context.ActivePlayer.Name} has untapped {untappedCount} Cards");
    }

    public void HandleAction(GameContext context, PlayerAction action) 
    {
        context.TransitionTo(new UpkeepStep());
    }

    public void OnStepExit(GameContext context)
    {

    }
}
