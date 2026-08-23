using MTG.Core.Enums;

namespace MTG.Engine.TurnSteps;

public class EndOfCombatStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.EndOfCombat;
}