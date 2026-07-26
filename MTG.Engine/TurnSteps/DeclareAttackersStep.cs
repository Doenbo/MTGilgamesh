using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class DeclareAttackersStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.DeclareAttackers;
}