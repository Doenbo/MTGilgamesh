using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class MainStep : TurnStepBase
{
    public override TurnStep Name { get; init; }

    public MainStep(TurnStep ts)
    {
        Name = ts;
    }

    public override bool CanPlaySorcerySpeed(GameContext context, CommanderPlayer player)
    {
        return context.ActivePlayer == player;
    }
}
