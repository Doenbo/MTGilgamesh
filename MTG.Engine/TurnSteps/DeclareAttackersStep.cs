using MTG.Core.Enums;

namespace MTG.Engine.TurnSteps;

public class DeclareAttackersStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.DeclareAttackers;
}