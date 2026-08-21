using MTG.Engine.Enums;

namespace MTG.Engine.TurnSteps;

public class CombatBeginStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.CombatBegin;
}
