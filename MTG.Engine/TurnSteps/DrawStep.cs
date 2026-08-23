using MTG.Core.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class DrawStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.Draw;

    protected override void PerformTurnBasedActions(GameContext context)
    {
        context.ActivePlayer.DrawCard();
        context.Display.LogInfo($"{context.ActivePlayer.Name} has drawn a card for turn.");
    }
}
