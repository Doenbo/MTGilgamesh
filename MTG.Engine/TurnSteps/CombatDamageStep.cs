using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class CombatDamageStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.CombatDamage;
}