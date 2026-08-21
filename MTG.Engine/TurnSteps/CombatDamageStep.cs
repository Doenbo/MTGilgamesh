using MTG.Engine.Enums;

namespace MTG.Engine.TurnSteps;

public class CombatDamageStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.CombatDamage;
}