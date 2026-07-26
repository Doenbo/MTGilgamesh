using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class CombatBeginStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.CombatBegin;
}
