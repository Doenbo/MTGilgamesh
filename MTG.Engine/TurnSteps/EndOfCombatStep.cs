using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class EndOfCombatStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.EndOfCombat;
}